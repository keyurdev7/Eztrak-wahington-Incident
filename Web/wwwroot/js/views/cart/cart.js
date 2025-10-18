$(function () {
    $("body").off("click", ".submit-cart")
    $("body").on("click", ".submit-cart", function () {
        if ($(".cart-create-form").valid()) {
            var data = $(".cart-create-form").serialize();
            $.ajax({
                type: "POST",
                data: data,
                url: "/Order/Create",
                success: function (response) {
                    if (response.Status) {
                        window.location = "/Inventories?activeTab=orders";
                    }
                    else {
                        alert(response.Message);
                    }
                },
                error: function (e) {
                }
            });
        }
    });
});

function addInventoryToCart(btn, invid) {

    //add to cart
    var ajaxLoaderFullScreen = $(".ajax-loader-full-screen");
    ajaxLoaderFullScreen.show();
    var _btn = $(btn);
    var urlString = "/Cart/AddInventoryToCart";
    urlString = urlString + "?id=" + invid;
    $.ajax({
        type: "POST",
        url: urlString,
        dataType: "json",
        success: function (response) {
            //_btn.text('Added To Cart');
            _btn.addClass('disabled');
            _btn.find('i').attr('title', 'Added to Cart');
            _btn.attr('disabled', 'disabled');
            _btn.removeAttr('onclick');

            fnUpdateShoppingCartLayout();
            ajaxLoaderFullScreen.hide();
        },
        error: function (e) {
            ajaxLoaderFullScreen.hide();

        }
    });
}

function addEquipmentToCart(btn, invid) {

    //add to cart
    var ajaxLoaderFullScreen = $(".ajax-loader-full-screen");
    ajaxLoaderFullScreen.show();
    var _btn = $(btn);
    var urlString = "/Cart/AddEquipmentToCart";
    urlString = urlString + "?id=" + invid;
    $.ajax({
        type: "POST",
        url: urlString,
        dataType: "json",
        success: function (response) {
            //_btn.text('Added To Cart');
            _btn.addClass('disabled');
            _btn.find('i').attr('title', 'Added to Cart');
            _btn.attr('disabled', 'disabled');
            _btn.removeAttr('onclick');

            fnUpdateEquipmentShoppingCartLayout();
            ajaxLoaderFullScreen.hide();
        },
        error: function (e) {
            ajaxLoaderFullScreen.hide();

        }
    });
}

function deleteInventoryFromCart(inventoryId, element) {
    var _element = element;
    var urlString = `/Cart/DeleteInventoryFromCart/${inventoryId}`;
    $.ajax({
        type: "POST",
        url: urlString,
        dataType: "json",
        success: function (response) {
            if (response) {
                $(_element).closest("tr").remove();
            }
        }
    });
}

function deleteEquipmentFromCart(equipmentId, element) {
    var _element = element;
    var urlString = `/Cart/DeleteEquipmentFromCart/${equipmentId}`;
    $.ajax({
        type: "POST",
        url: urlString,
        dataType: "json",
        success: function (response) {
            if (response) {
                $(_element).closest("tr").remove();
            }
        }
    });
}

function fnUpdateShoppingCartLayout() {
    //update shopping cart
    $.ajax({
        type: "GET",
        url: "/Cart/ShowShoppingCart",
        dataType: "html",
        success: function (content) {
            $("#divshoppingcartlayout").empty().html(content);
        },
        error: function (e) { }
    });
}

function fnUpdateEquipmentShoppingCartLayout() {
    //update shopping cart
    $.ajax({
        type: "GET",
        url: "/Cart/ShowShoppingCart",
        dataType: "html",
        success: function (content) {
            $("#divshoppingcartlayout").empty().html(content);
        },
        error: function (e) { }
    });
}

function addToCartSelectedItems() {
    var selectedData = inventoryGrid.rows('.selected').data();
    var ids = [];
    if (selectedData.length > 0) {
        $.each(selectedData, function (i, item) {
            ids.push(item.InventoryId);
        });
        $('#loading').show();
        $.ajax({
            url: url_AjaxAddToCartSelectedInventory,
            type: "POST",
            data: { inventoryIds: ids.join() },
            success: function (data) {
                if (data) {
                    SwalSuccessAlert('Selected inventories are added to cart Successfully.');
                }
                else {
                    SwalErrorAlert("Add To Cart failed ! Error try again.");
                }
                $('#loading').hide();
            },
            complete: function () {
                fnUpdateShoppingCartLayout();
                $('#loading').hide();
            },
            error: function (r, s, t) {
                SwalErrorAlert('Add To Cart failed ! Error try again.');
                $('#loading').hide();
            }
        });
    }
    else {
        SwalErrorAlert('Please select at least one record.');
    }
}

function afterWorkOrderSelect(recordId, modelPropertyName) {
    //let model = $("#work-order-id").select2('data').filter(function (e) {
    //    return e.id == recordId;
    //})[0].additionalAttributesModel;
    //$("#WorkOrderDescription").html(model.Description);
    //$("#WorkOrderPriority").html(model.Urgency);
}

function afterWorkOrderUnSelect(recordId, modelPropertyName) {
    $("#WorkOrderDescription").html("-");
    $("#WorkOrderPriority").html("-");

}
