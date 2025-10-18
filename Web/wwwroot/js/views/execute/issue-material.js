

$('#btnSaveTransactions').click(function (e) {
    e.preventDefault();
    fnSaveRestageInventory();
});



function registerLocationSelect2(index) {
    var locationId = "#NewLocationId_" + index;
    var oldLocationId = "#OldLocationId_" + index;
    var ignoreIds = [];
    ignoreIds.push($(oldLocationId).val());
    SetLocationSelect2(locationId, true, ignoreIds);
}


function fnIsIssueInventoryItemValid() {
    var allCheckedItemsValid = true;
    var quantity = 0;
    $('.issue-material-quantity').each(function () {
        quantity += parseInt($(this).val()) || 0;
    });
    var orderedQuantity = parseInt($("#hdnOrderedQty").val());

    // Check if location and quantity are set for the current checked item
    if (isNaN(quantity) || quantity <= 0 || quantity != orderedQuantity) {
        allCheckedItemsValid = false;
        return false; // Break out of the loop
    }
    return allCheckedItemsValid;
}

function fnIsIssueEquipmentItemValid() {
    var allCheckedItemsValid = true;
    var quantity = 0;
    $('.issue-material-quantity').each(function () {
        quantity += parseInt($(this).val()) || 0;
    });
    var orderedQuantity = parseInt($("#hdnOrderedQty").val());

    // Check if location and quantity are set for the current checked item
    if (isNaN(quantity) || quantity <= 0 || quantity != orderedQuantity) {
        allCheckedItemsValid = false;
        return false; // Break out of the loop
    }
    return allCheckedItemsValid;
}

function SaveIssueInventoryItem() {
    var isValid = fnIsIssueInventoryItemValid();
    if (isValid) {
        let formData = getSelectedTransactionItems();
        var orderId = $("#hdn-order-id").val();
        $.ajax({
            url: "/Order/IssueInventoryItem",
            type: "POST",
            processData: false, // Ensure that jQuery does not process the FormData object
            contentType: false, // Ensure that jQuery does not set the Content-Type header
            data: formData, // Use the FormData object directly as the data
            success: function (response) {
                if (response) {
                    SwalSuccessAlert("Successfully issued materials.");
                    $("#issue-material-modal").find(".modal-title").html('');
                    $("#issue-material-modal").find(".modal-body").html('');
                    $("#issue-material-modal").modal("hide");
                    setTimeout(function () {
                        dataTableManager.searchDataTable();
                        loadUpdateAndDetailModalPanel(`/Order/Detail/${orderId}`);
                    }, 500);
                }
                else {
                    SwalErrorAlert("Error: Unable to Issue items. Please try again.");
                }
            },
            complete: function () {
            },
            error: function (r, s, t) {
                SwalErrorAlert('Please try again.');
            }
        });
    }
    else {
        SwalErrorAlert('Quantity must be equal to ordered quantity.');
    }
}

function SaveIssueEquipmentItem() {
    var isValid = fnIsIssueEquipmentItemValid();
    if (isValid) {
        let formData = getSelectedTransactionItems();
        var orderId = $("#hdn-order-id").val();
        $.ajax({
            url: "/Order/IssueEquipmentItem",
            type: "POST",
            processData: false, // Ensure that jQuery does not process the FormData object
            contentType: false, // Ensure that jQuery does not set the Content-Type header
            data: formData, // Use the FormData object directly as the data
            success: function (response) {
                if (response) {
                    SwalSuccessAlert("Successfully issued equipments.");
                    $("#issue-material-modal").find(".modal-title").html('');
                    $("#issue-material-modal").find(".modal-body").html('');
                    $("#issue-material-modal").modal("hide");
                    setTimeout(function () {
                        dataTableManager.searchDataTable();
                        loadUpdateAndDetailModalPanel(`/Order/Detail/${orderId}`);
                    }, 500);
                }
                else {
                    SwalErrorAlert("Error: Unable to Issue items. Please try again.");
                }
            },
            complete: function () {
            },
            error: function (r, s, t) {
                SwalErrorAlert('Please try again.');
            }
        });
    }
    else {
        SwalErrorAlert('Quantity must be equal to ordered quantity.');
    }
}

function getSelectedTransactionItems() {
    var formData = new FormData();
    $('.issue-material-table tbody tr').each(function (index, v) {
        $(v).find('select, input').each(function (ind, val) {
            var fieldName = $(val).attr('name');
            if (fieldName != undefined) {
                let fieldValue = $(val).val();
                let newFieldName = fieldName.replace(/\[\d+\]/, '[' + index + ']');
                formData.append(newFieldName, fieldValue);
            }
        })
    });

    $(".issue-material-data").each(function (i, v) {
        var fieldName = $(v).attr('name');
        var fieldValue = $(v).val();
        formData.append(fieldName, fieldValue);
    });
    return formData;
}

function initializeLocationSelect2(locationIds) {
    locationIds.each(function () {
        let id = "#" + $(this).attr('id');
        SetLocationSelect2(id);
    });
}