$(function () {
    window.addEventListener('createAssetModal', (event) => {
        // debugger;
        loadUpdateAndDetailModalPanel("/Asset/Create?lat=" + event.detail.lat + "&long=" + event.detail.lng);
    });
    window.addEventListener('detailAssetModal', (event) => {
        // debugger;
        loadUpdateAndDetailModalPanel("/Asset/Update/" + event.detail.id);
    });

    $(document).off('click', '.delete');
    $(document).on('click', '.delete', (e) => {
        e.preventDefault();
        let deleteObj = {
            deleteUrl: '',
            confirmBtnText: "",
            cancelBtnText: "",
            deleteReturnUrl: ""
        };
        // Check if the clicked element or its ancestor is an <a> element
        let $deleteLink = $(e.target).closest('a');
        if ($deleteLink.length > 0) {
            deleteObj.deleteUrl = $deleteLink.attr('href');
        }
        DeleteDataItem(deleteObj, this);
    });
});
function onUpdateRecordSuccess(recordId, modalPanelId, form) {
    let url = form.action;
    const isUpdate = url.includes("/Update") || url.toLowerCase().includes("/update");
    disableControls(form);
    $.ajax({
        url: "/Map/Get",
        data: { id: recordId },
        success: function (response) {
            enableControls(form);
            $("#" + modalPanelId).modal("toggle");
            if (isUpdate) {
                const assetUpdatedEvent = new CustomEvent('assetUpdated', {
                    detail: response
                });
                window.dispatchEvent(assetUpdatedEvent);
            }
            else {
                const assetCreatedEvent = new CustomEvent('assetCreated', {
                    detail: response
                });
                window.dispatchEvent(assetCreatedEvent);
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            swal.fire({
                title: 'Error!',
                text: 'Something went wrong. Please try again later.',
                type: 'error',
                showCancelButton: true,
                confirmButtonText: "Okay",
                cancelButtonText: "Cancel",
                confirmButtonClass: 'btn btn-success m-2',
                cancelButtonClass: 'btn btn-danger',
                buttonsStyling: false
            });
            enableControls(form);
            $("#" + modalPanelId).modal("toggle");
        }
    });
}
