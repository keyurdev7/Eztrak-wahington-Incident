function initializeDataCard(id) {
    let cardDataContainer = $("#" + id).find(".card-data");
    const url = $(cardDataContainer).data("url");
    $(cardDataContainer).append("<button style='display:none' type='button' class='loader btn bg-custom-dark btn-float rounded-round'><i class='icon-spinner4 spinner'></i></button>");
    showDashboardCardLoader(cardDataContainer);
    $.ajax({
        url: url,
        dataType: "html", // Expecting HTML data from the server
        success: function (data) {
            cardDataContainer.html(data);
            hideDashboardCardLoader(cardDataContainer);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideDashboardCardLoader(cardDataContainer);
            console.error("Error loading data:", textStatus, errorThrown);
            // Handle errors appropriately (e.g., display an error message)
        }
    });
}


function showDashboardCardLoader(element) {
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
function hideDashboardCardLoader(element) {
    $(element).unblock();
}
