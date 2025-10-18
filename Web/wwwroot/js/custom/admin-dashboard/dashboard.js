$(function () {
    updateSelectedFilters();
    GetChartData();
    //initializing branch select2
  
    $(document).off('click', '#search-filters');
    $(document).on('click', '#search-filters', function () {
        $(".search-filter-container").show();
        $(".chart-container").hide();
    });
    $(document).off('click', '.search-form-btn');
    $(document).on('click', '.search-form-btn', function () {
        updateSelectedFilters();
        $(".chart-container").show();
        GetChartData();
        $(".search-filter-container").hide();
       
    });

    $(document).off('click', '#back-to-list');
    $(document).on('click', '#back-to-list', function () {
        $(".search-filter-container").hide();
        updateSelectedFilters();
        $(".chart-container").show();
    });
    $(document).off('click', '.clear-form-btn');
    $(document).on('click', '.clear-form-btn', function () {
        $('#filter-form').trigger("reset");
        $(".select-search").each(function (i, element) {
            $(element).val('').trigger('change');
        });
        updateSelectedFilters();
    });
});
function AddChartHtml(id, title) {
    var html = `<div class="col-md-6">
        <div class="card">
            <div class="card-header">
                <h4>${title}</h4>
            </div>
            <div class="card-body">
                <div class="chartdiv" id="${id}"></div>
            </div>
        </div>
    </div>`;
    $("#chart-data-container").append(html);
}

function GetChartData() {
    try {
        showLoader();
        var fromDate = $("#from-date").val();
        var toDate = $("#to-date").val();
        $.ajax({
            url: '/Home/GetChartData',
            data: { "fromDate": fromDate, "toDate": toDate },
            type: "GET",
            success: function (res) {
                $("#chart-data-container").html("");
                $.each(res, function (i, v) {
                    
                    AddChartHtml(v.ElementId, v.Title);
                    GeneratePieChart(v.ElementId, v.Data);
                   
                });
            },
            error: function (xhr, status, error) {
                hideLoader();
                console.error("AJAX error:", status, error);
            }
        });

    } catch (outerError) {
        console.error("Outer try-catch block error:", outerError);
    }
}
function GeneratePieChart(id, data) {
    // Themes begin
    am4core.useTheme(am4themes_animated);
    // Themes end

    // Create chart instance
    var chart = am4core.create(id, am4charts.PieChart);

    // Add and configure Series
    var pieSeries = chart.series.push(new am4charts.PieSeries());
    pieSeries.dataFields.value = "value";
    pieSeries.dataFields.category = "name";

    // Let's cut a hole in our Pie chart the size of 30% the radius
    chart.innerRadius = am4core.percent(30);

    // Put a thick white border around each Slice
    pieSeries.slices.template.stroke = am4core.color("#fff");
    pieSeries.slices.template.strokeWidth = 2;
    pieSeries.slices.template.strokeOpacity = 1;
    pieSeries.slices.template
        // change the cursor on hover to make it apparent the object can be interacted with
        .cursorOverStyle = [
            {
                "property": "cursor",
                "value": "pointer"
            }
        ];

    pieSeries.alignLabels = false;
    pieSeries.labels.template.bent = true;
    pieSeries.labels.template.radius = 10;
    pieSeries.labels.template.padding(0, 0, 0, 0);
    pieSeries.labels.template.maxWidth = 80;
    pieSeries.labels.template.truncate = true;
    pieSeries.labels.template.text = "{name}";

    // Create a base filter effect (as if it's not there) for the hover to return to
    var shadow = pieSeries.slices.template.filters.push(new am4core.DropShadowFilter);
    shadow.opacity = 0;

    // Create hover state
    var hoverState = pieSeries.slices.template.states.getKey("hover"); // normally we have to create the hover state, in this case it already exists

    // Slightly shift the shadow and make it more prominent on hover
    var hoverShadow = hoverState.filters.push(new am4core.DropShadowFilter);
    hoverShadow.opacity = 0.7;
    hoverShadow.blur = 5;

    // Add a legend
    chart.legend = new am4charts.Legend();


    chart.data = data;
}

// Function to update the content of the chart-container
function updateSelectedFilters() {
    let fromDate = $("#from-date").val();
    let toDate = $("#to-date").val();

    if (!fromDate) {
        fromDate = "Begining";
    }
    else {
        fromDate = formatDate(fromDate);
    }
    if (!toDate) {
        toDate = "Till Date";
    }
    else {
        toDate = formatDate(toDate);
    }

   
    // Example: You can update the content based on the selected filters
    let content = "";
    content += "<div class='filter-item period'>" + fromDate + "<span class='period-space'> - </span>" + toDate + "</div>";

    // Update the content in the chart-container
    $(".selected-filters").html(content);
}
function formatDate(dateString) {
    let options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
    console.log(dateString);
    let formattedDate = new Date(dateString).toLocaleDateString('en-US', options);
    return formattedDate;
}
function showLoader() {
    // Create the loader element
    let loader = $(
        "<div class='loader-container d-flex flex-column align-items-center justify-content-center'>" +
        "<div class='row'>" +
        "<div class='spinner-border text-custom' role='status'>" +
        "<span class='sr-only'>Loading...</span>" +
        "</div>" +
        "</div>" +
        "<div class='row mt-3'>" +
        "<strong class='text-custom'>Loading Dashboard...</strong>" +
        "</div>" +
        "</div>"
    );

    // Append the loader to the chart container
    $("#chart-data-container").html(loader);
}



