$(function () {
    loadAssetRadios();

    $(document).off("change", "#asset-type-id");
    $(document).on("change", "#asset-type-id", function () {
        loadAssetRadios();
    });
});

function loadAssetRadios() {
    var assetTypeId = $("#asset-type-id").select2('data')[0].id;
    var assetId = $("#asset-id").val();
    $.ajax({
        type: "GET",
        url: `/Asset/GetAssetTypeHtml?assetTypeId=${assetTypeId}&assetId=${assetId}`,
        dataType: "html",
        success: function (content) {
            $("#asset-type-level-radio").empty();
            $("#asset-type-level-radio").html(content);
        },
        error: function (e) { }
    });
}

function ProcessFormData(formData) {
    $(".asset-type-association-radio:checked").each(function (i, v) {
        var assetId = $("#asset-id").val();
        var assetTypeId = $("#asset-type-id").select2('data')[0].id;
        var assetTypeLevel1Id = $(v).attr("attr-level1-id");
        var assetTypeLevel2Id = $(v).val();

        formData.append(`AssetAssociations[${i}].Asset.Id`, assetId);
        formData.append(`AssetAssociations[${i}].AssetType.Id`, assetTypeId);
        formData.append(`AssetAssociations[${i}].AssetTypeLevel1.Id`, assetTypeLevel1Id);
        formData.append(`AssetAssociations[${i}].AssetTypeLevel2.Id`, assetTypeLevel2Id);
    });
}