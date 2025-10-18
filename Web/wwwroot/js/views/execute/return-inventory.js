

$('#btnSaveTransactions').click(function (e) {
    e.preventDefault();
    fnSaveReturnInventory();
});


function fnIsRemoveInventoryValid() {
    var allCheckedItemsValid = true;
    $('.transactions-checkbox:checked').each(function () {
        var index = $(this).closest('tr').index(); // Get the index of the row
        var quantity = parseInt($('input[name="Transactions[' + index + '].ReturnedQuantity"]').val()); // Get the entered quantity

        // Check if location and quantity are set for the current checked item
        if (isNaN(quantity) || quantity <= 0) {
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
        $.get("/Execute/GetItemsToReturn?lotNo=" + lotNo, function (data, status) {
            current_tab = $(".step-card:visible");
            next_tab = $(".step-card:visible").next();
            step_no = $(this).parents(".staging-steps").find("fieldset").index(next_tab);
            step_no++;
            $('.step-no').html("3");
            next_tab.show();
            current_tab.hide();
            $("#inventory-table-container").html(data);
        });
    }
    else {
        spnLotNumber.html("Scan PO # is required.");
    }
}

function fnSaveReturnInventory() {
    var anySelected = $('.transactions-checkbox:checked').length > 0;
    if (anySelected == false) {
        SwalErrorAlert('Please select at least one record.');
        return;
    }
    var isValid = fnIsRemoveInventoryValid();
    if (isValid) {
        let formData = getSelectedTransactionItems();
        $.ajax({
            url: "/Execute/ReturnInventoryItems",
            type: "POST",
            processData: false, // Ensure that jQuery does not process the FormData object
            contentType: false, // Ensure that jQuery does not set the Content-Type header
            data: formData, // Use the FormData object directly as the data
            success: function (response) {
                if (response) {
                    SwalSuccessAlert("Successfully returned inventory items.");
                    window.location = "/Inventories?activeTab=execute";
                }
                else {
                    SwalErrorAlert("Error: Unable to Return inventory items. Please try again.");
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
        SwalErrorAlert('Please make sure returned quantity is valid for all checked items, and returned quantity is greater than 0.');
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
