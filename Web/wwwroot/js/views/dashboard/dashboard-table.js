function initializeDashboardTable(id) {
    let table = $("#" + id);
    let tableDataContainer = $("#" + id).closest(".card-body");
    const url = $(table).data("url");
    $(tableDataContainer).append("<button style='display:none' type='button' class='loader btn bg-custom-dark btn-float rounded-round'><i class='icon-spinner4 spinner'></i></button>");
    showDashboardTableLoader(tableDataContainer);
    $.ajax({
        url: url,
        dataType: "html", // Expecting HTML data from the server
        success: function (data) {
            table.html(data);
            hideDashboardCardLoader(tableDataContainer);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideDashboardCardLoader(tableDataContainer);
            console.error("Error loading data:", textStatus, errorThrown);
            // Handle errors appropriately (e.g., display an error message)
        }
    });
}


function showDashboardTableLoader(element) {
    let loader = $(element).find(".loader");
    if ($(element).length > 0) {
        $(element).block({
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
function hideDashboardTableLoader(element) {
    $(element).unblock();
}
