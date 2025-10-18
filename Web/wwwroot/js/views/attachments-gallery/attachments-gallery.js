

function ViewAttachmentGrid(id, url) {
    $.ajax({
        url: url,
        data: { 'id': id },
        type: 'post',
        success: function (data) {
            $("#attachment-detail-modal-body").html(data);
        }
    });
}
function GetAttachmentUrl(id, url) {
    $.ajax({
        url: url,
        data: { 'id': id },
        type: 'get',
        success: function (data) {
            window.open(data, '_blank');;
        }
    });
}