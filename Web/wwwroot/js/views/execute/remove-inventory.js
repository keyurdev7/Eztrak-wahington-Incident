

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


function fnIsRemoveInventoryValid() {
    var allCheckedItemsValid = true;
    $('.transactions-checkbox:checked').each(function () {
        var index = $(this).closest('tr').index(); // Get the index of the row
        var justification = $('select[name="Transactions[' + index + '].Justification"]').val(); // Get the selected justification value
        var quantity = parseInt($('input[name="Transactions[' + index + '].RemovedQuantity"]').val()); // Get the entered quantity

        // Check if location and quantity are set for the current checked item
        if (!justification || isNaN(quantity) || quantity <= 0) {
            allCheckedItemsValid = false;
            return false; // Break out of the loop
        }
    });
    return allCheckedItemsValid;
}

function GetTransactions(lotNo) {

    var spnLotNumber = $('#spnLotNumber');
    spnLotNumber.empty();
    if (lotNo.length > 0) {
        $('#ajax-loader').show();
        $.get("/Execute/GetItemsToRemove?lotNo=" + lotNo, function (data, status) {
            current_tab = $(".step-card:visible");
            next_tab = $(".step-card:visible").next();
            step_no = $(this).parents(".staging-steps").find("fieldset").index(next_tab);
            step_no++;
            $('.step-no').html("3");
            next_tab.show();
            current_tab.hide();
            $("#inventory-table-container").html(data);
            initializeLocationSelect2($('[id^="location-id"]'));

        });
    }
    else {
        spnLotNumber.html("Scan PO # is required.");
    }
}

function fnSaveRestageInventory() {
    var anySelected = $('.transactions-checkbox:checked').length > 0;
    if (anySelected == false) {
        SwalErrorAlert('Please select at least one record.');
        return;
    }
    var isValid = fnIsRemoveInventoryValid();
    if (isValid) {
        let formData = getSelectedTransactionItems();
        $.ajax({
            url: "/Execute/RemoveInventoryItems",
            type: "POST",
            processData: false, // Ensure that jQuery does not process the FormData object
            contentType: false, // Ensure that jQuery does not set the Content-Type header
            data: formData, // Use the FormData object directly as the data
            success: function (response) {
                if (response) {
                    SwalSuccessAlert("Successfully Removed inventory items.");
                    window.location = "/Inventories?activeTab=execute";
                }
                else {
                    SwalErrorAlert("Error: Unable to Remove inventory items. Please try again.");
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
        SwalErrorAlert('Please make sure justification and quantity are valid for all checked items, and quantity is greater than 0.');
    }
}

function getSelectedTransactionItems() {
    var formData = new FormData();
    $('.transactions-checkbox:checked').each(function (index) {
        //var index = $(this).closest('tr').index(); // Get the index of the row

        // Iterate through selects and inputs of the current row
        $(this).closest('tr').find('select, input').each(function () {
            var fieldName = $(this).attr('name');
            if (fieldName != undefined) {
                var fieldValue = $(this).val();
                let newFieldName = fieldName.replace(/\[\d+\]/, '[' + index + ']');
                formData.append(newFieldName, fieldValue);
            }
        });
    });
    return formData;
}
function initializeLocationSelect2(locationIds) {
    locationIds.each(function () {
        let id = "#" + $(this).attr('id');
        SetLocationSelect2(id);
    });
}