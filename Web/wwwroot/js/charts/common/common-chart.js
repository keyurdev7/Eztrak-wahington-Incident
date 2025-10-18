var showSelectedFilters = true;

$(function () {
    $(document).off('click', '.clear-chart-form-btn');
    $(document).on('click', '.clear-chart-form-btn', function () {
        ClearChartSearch(this);
    });

    $(document).off('keypress', '.chart-container input[type=search],#filter-form input[type=text]');
    $(document).on('keypress', '.chart-container input[type=search],#filter-form input[type=text]', function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            event.preventDefault();
            FilterChart(this);

        }
    });

    $(document).off('click', '.badge-chart-clear');
    $(document).on('click', '.badge-chart-clear', function () {
        ClearChartSearch(this);
    });

    $(document).off('click', '.badge-chart-search');
    $(document).on('click', '.badge-chart-search', function () {
        var inputName = $(this).attr('data-input-name');
        RemoveSearchFilterInput(this, inputName);
    });

    $(document).off('click', '.search-chart-form-btn');
    $(document).on('click', '.search-chart-form-btn', function () {
        FilterChart(this);
    });
    $(document).off('click', '.search-filters-chart');
    $(document).on('click', '.search-filters-chart', function () {
        let searchFilterContainer = GetChartSearchFilterContainer(this);
        let charContentContainer = GetChartContentContainer(this);
        $(charContentContainer).hide();
        $(searchFilterContainer).show();
    });
    $(document).off('click', '.back-to-chart');
    $(document).on('click', '.back-to-chart', function () {
        ShowChartContentAndHideFilters(this);

    });
});

function InitializeChart(id) {
    let loaderElement = GetChartContentContainerCardBody($("#" + id));

    $(loaderElement).append("<button style='display:none' type='button' class='loader btn bg-custom-dark btn-float rounded-round'><i class='icon-spinner4 spinner'></i></button>");

    FilterChart($("#" + id));
}


function SetSearchFilters(element) {

    if (showSelectedFilters) {
        let charContentContainer = GetChartContentContainer(element);
        let chartSearchFilterContainer = GetChartSearchFilterContainer(element);
        let chartSelectedFilterContainer = GetChartSelectedFilterContainer(element);
        let containerHtml = "";
        let toolTipHtml = "";
        let badgesContainerRemainingWidth = $(chartSelectedFilterContainer).width();
        let truncatedFilterBadge = "<span class='badge badge-chart-search mb-1' data-bs-popup='tooltip' data-bs-html='true' data-bs-placement='top' #tooltiptitle#>...</span>";
        let truncatedBadgeWidth = calculateContentWidth(truncatedFilterBadge, "badge-chart-search");
        let clearFilterBadge = "<span class='chart-clear-container'><span class='badge badge-chart-clear fas mb-1'>Clear</span></span>";
        let clearFilterBadgeWidth = calculateContentWidth(clearFilterBadge, "chart-clear-container");
        badgesContainerRemainingWidth -= clearFilterBadgeWidth;
        badgesContainerRemainingWidth -= 50;//Adjusting variance

        let truncateBadges = false;

        $(chartSearchFilterContainer).find('input, select').each(function (index) {
            let input = $(this);
            if (input.attr('type') != "hidden" && input.val() != "") {
                let value = input.val();
                if ($(input).data('select2')) {
                    value = $(input).select2('data')[0].text
                }
                else if ($(input).is('select')) {
                    value = $(input).find(":selected").text();
                }
                else if ($(input).is(':checkbox')) {
                    value = $(input).is(":checked");
                }
                let inputName = input.attr('name');
                let badgeContent = "<span class='badge badge-chart-search mb-1' data-input-name='" + inputName + "'>" + $(input).parent().find("label").html() + " : " + value + "</span>";
                if (containerHtml == "") {
                    containerHtml = "<span class='me-1'>Filters: </span>";
                    toolTipHtml += containerHtml;
                }
                if (!truncateBadges) {
                    let badgeWidth = calculateContentWidth(badgeContent, "badge-chart-search");
                    if (badgeWidth <= badgesContainerRemainingWidth) {
                        badgesContainerRemainingWidth = badgesContainerRemainingWidth - badgeWidth; // Adjust factor (10) based on average character width
                        if (truncatedBadgeWidth >= badgesContainerRemainingWidth) {
                            truncateBadges = true;
                        }
                        else {
                            containerHtml += badgeContent;
                        }

                    }
                    else {
                        truncateBadges = true;
                    }
                }


                toolTipHtml += badgeContent;
            }
        });
        if (truncateBadges) {
            let tooltipContainer = "<div class='d-flex flex-wrap w-100'>" + toolTipHtml + "</div>";
            truncatedFilterBadge = truncatedFilterBadge.replace("#tooltiptitle#", 'data-bs-original-title="' + tooltipContainer.replaceAll("\"", "'") + '"');
            containerHtml += truncatedFilterBadge;
        }
        if (containerHtml != "") {
            containerHtml += clearFilterBadge;
            containerHtml = "<div class='d-flex flex-wrap w-100'>" + containerHtml + "</div>";
        }
        $(chartSelectedFilterContainer).html(containerHtml);
        if (truncateBadges) {
            try {
                $(chartSelectedFilterContainer).find('[data-bs-popup="tooltip"]').tooltip();
            }
            catch {

            }
        }
    } else {
        $(chartSelectedFilterContainer).html("");
    }

}

function RemoveSearchFilterInput(element, inputName) {
    let searchForm = GetChartSearchForm(element);
    var input = $(searchForm).find("input[name='" + inputName + "']");
    if ($(input).data('select2')) {
        $(input).val('').trigger('change');
    }
    else {
        $(input).val("");
    }
    FilterChart(element);
}
function ClearChartSearch(element) {
    let searchForm = GetChartSearchForm(element);
    $(searchForm).trigger("reset");
    // Clearing select inputs with Select2 plugin within the searchForm
    $(searchForm).find('select[class*="select2"]').each(function (i, search2Element) {
        $(search2Element).val(null).trigger('change');
    });
    FilterChart(element);
}

function FilterChart(element) {
    ShowChartContentAndHideFilters(element);
    LoadChartData(element);
    SetSearchFilters(element);
}

function LoadChartData(element) {
    // debugger;
    let chartContainer = GetChartContainer(element);
    let chartDataContainer = GetChartDataContainer(element);
    let chartDataUrl = GetChartDataUrl(element);
    let chartSearchFormData = GetSerialzedChartSearchFormData(element);
    let chartId = GetChartId(element);
    let title = GetChartTitle(element);
    let chartGenerationFunction = GetChartGenerationFunction(element);
    chartDataContainer.html("");
    showChartLoader(chartContainer);
    $.ajax({
        url: chartDataUrl,
        type: 'GET',
        data: chartSearchFormData,
        success: function (response) {
            if (response.IsSuccess) {
                if (typeof window[chartGenerationFunction] === "function") {
                    window[chartGenerationFunction](chartId, response.Data, title);
                    // Save response.Data in local storage keyed by chartId
                    window.localStorage.setItem('chartData_' + chartId, JSON.stringify(response.Data));
                } else {
                    console.error('Function', chartGenerationFunction, 'not found or not a function');
                }
            }
            else {
                chartDataContainer.html(getErrorMessageContent(response.Message));
            }
            hideChartLoader(chartContainer);
        },
        error: function (xhr, status, error) {
            hideChartLoader(chartContainer);
            // Handle errors here
            console.error('AJAX call error:', error);
        }
    });
}
function GetChartContainer(element) {
    return $(element).closest(".chart-container");
}

function GetChartSearchForm(element) {
    let chartContainer = GetChartContainer(element);
    return $(chartContainer).find("form");
}
function GetChartSearchFilterContainer(element) {
    let chartContainer = GetChartContainer(element);
    return $(chartContainer).find(".chart-search-filter-container");
}
function GetChartSelectedFilterContainer(element) {
    let chartContainer = GetChartContainer(element);

    return $(chartContainer).find(".chart-selected-filters-container");
}
function GetChartContentContainerCardBody(element) {
    let chartContentContainer = GetChartContentContainer(element);
    return $(chartContentContainer).closest(".card-body");
}
function GetChartContentContainer(element) {
    let chartContainer = GetChartContainer(element);
    return $(chartContainer).find(".chart-content-container");
}

function GetChartDataContainer(element) {
    let chartContainer = GetChartContainer(element);
    return $(chartContainer).find(".chart-data-container");
}

function ShowChartContentAndHideFilters(element) {
    let searchFilterContainer = GetChartSearchFilterContainer(element);
    let charContentContainer = GetChartContentContainer(element);
    $(searchFilterContainer).hide();
    $(charContentContainer).show();
}

function GetChartDataUrl(element) {
    let chartContainer = GetChartContainer(element);
    return $(chartContainer).find(".chart-data-container").data("url");
}
function GetChartId(element) {
    let chartContainer = GetChartContainer(element);
    return $(chartContainer).find(".chart-data-container").attr("id");
}
function GetChartTitle(element) {
    let chartContainer = GetChartContainer(element);
    return $(chartContainer).find(".chart-data-container").data("title");
}
function GetChartGenerationFunction(element) {
    let chartContainer = GetChartContainer(element);
    return $(chartContainer).find(".chart-data-container").data("chart-generation-function");
}
function GetSerialzedChartSearchFormData(element) {
    let chartSearchForm = GetChartSearchForm(element);
    let serializedData = $(chartSearchForm).serialize();
    return serializedData;
}

function calculateContentWidth(content, containerClassName) {
    // Create temporary element
    let tempContainer = document.createElement('div');
    tempContainer.id = 'temp-container';
    tempContainer.style.visibility = 'hidden';
    tempContainer.innerHTML = content;

    // Append to body (alternative: hidden container if needed)
    document.body.appendChild(tempContainer);

    // Get content width
    let contentWidth = tempContainer.querySelector("." + containerClassName).offsetWidth;

    // Remove temporary element
    document.body.removeChild(tempContainer);

    return contentWidth;
}

function showChartLoader(element) {
    let loaderElement = GetChartContentContainerCardBody(element);
    let loader = $(loaderElement).find(".loader");
    if ($(loaderElement).length > 0) {
        $(loaderElement).block({
            message: $(loader),
            centerY: false,
            centerX: false,
            css: {
                margin: 'auto',
                border: 'none',
                padding: '15px',
                backgroundColor: 'transparent',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                color: '#fff'
            }

        });
    }
}
function hideChartLoader(element) {
    let loaderElement = GetChartContentContainerCardBody(element);
    $(loaderElement).unblock();
}

function getErrorMessageContent(message) {
    // Create the container and errorContainer divs
    var containerDiv = $('<div>').addClass('container d-flex justify-content-center align-items-center').css('height', '100%');;
    var rowDiv = $('<div>').addClass('text-center').attr('id', 'errorContainer');

    // Generate the error sign HTML
    var errorHtml = `
          <div class="col-auto">
            <div class="error-sign"><i style="color: #9c3c3c;font-size: 5rem;" class="fas fa-exclamation-triangle"></i></div>
            <div class="mt-3" style="font-size: 1.2rem;">${message}</div>
          </div>
        `;

    // Append the error sign HTML to the rowDiv
    rowDiv.append(errorHtml);

    // Append the rowDiv to the containerDiv
    containerDiv.append(rowDiv);

    // Return the HTML content
    return containerDiv;
}

$(document).ready(function () {
    // Load chart data
    $('.chart-selection-type').on('change', function () {
        let id = $(this).data('id');
        let type = $(this).val();
        loadChartByType(type, id);
    });
}
);

function loadChartByType(type='bar', id) {
    // Get chart data from local storage
    let chartData = JSON.parse(window.localStorage.getItem('chartData_' + id));

    // Check if chart data is available
    if (chartData && type == 'bar') {
        // Generate chart
        GenerateAmBarChart(id, chartData);
    } 
    else if (chartData && type == 'pie') {
        // Generate chart
        GenerateAmPieChart(id, chartData);
    }
    else {
        // Load chart data
        LoadChartData($('#' + id));
    }

}
