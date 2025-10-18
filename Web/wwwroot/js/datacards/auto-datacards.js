var dataAjaxUrl = "";
$(function () {
    $(document).off('click', '.data-card');
    $(document).on('click', '.data-card', function (event) {
        if (!$(event.target).closest('.card-footer.card-action').length) {
            var viewLink = $(this).find('.view-link');
            if (viewLink.length > 0 && !$(event.target).hasClass('btn')) {
                window.location.href = viewLink.attr('href');
            }
        }
    });
    // Delete action
    $(document).off('click', '.delete-data-card');
    $(document).on('click', '.delete-data-card', (e) => {
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
        DeleteDataCardItem(deleteObj);
    });
});
function InitializeDataCards(url = "") {
    var formId = $(".search-form").attr('id');
    if (url === "") {
        var currentController = window.location.pathname.split('/')[1];
        var dataAjaxUrl = "/" + currentController + "/Search";
    }
    else {
        dataAjaxUrl = url;
    }
    $(document).off('click', '.clear-form-btn');
    $(document).on('click', '.clear-form-btn', function () {
        $('#' + formId).trigger("reset");
        $(".select-search").each(function (i, element) {
            $(element).val('').trigger('change');
        });
        SearchDataCard(dataAjaxUrl);
    });
    $(document).off('click', '.search-form-btn');
    $(document).on('click', '.search-form-btn', function () {
        SearchDataCard(dataAjaxUrl);
    });
    $(document).off('click', '#search-filters');
    $(document).on('click', '#search-filters', function () {
        $(".search-filter-container").show();
        $(".list-contatiner").hide();
    });
    $(document).off('click', '#back-to-list');
    $(document).on('click', '#back-to-list', function () {
        $(".search-filter-container").hide();
        $(".list-contatiner").show();
    });
    $(document).off('click', '.page-item');
    $(document).on('click', '.page-item', function (e) {
        e.preventDefault();
        if ($(this).hasClass("disabled") == false) {
            FilterDataCard(dataAjaxUrl, $(this).find('a').data('page'));
        }
    });
    $(document).off('click', '.delete');
    $(document).on('click', '.delete', function (e) {
        e.preventDefault();
        DeleteDataItem($(this).attr('href'));

    });

    FilterDataCard(dataAjaxUrl, 1);
}

function FilterDataCard(dataAjaxUrl, currentPage) {
    var listDiv = $(".list-content");
    var formId = $(".search-form").attr('id');
    var formData = $("#" + formId).serialize() + "&CurrentPage=" + currentPage;
    showLoader($(".list-container"));
    $.ajax({
        url: dataAjaxUrl,
        contentType: 'application/json',
        data: formData,
        success: function (res) {
            $(listDiv).html(res);
            hideLoader($(".list-container"));
        },
        error: (xhr, status, error) => {
            hideLoader($(".list-container"));
            console.error('AJAX Error:', error);
        }
    });
}

function SearchDataCard(dataAjaxUrl) {
    FilterDataCard(dataAjaxUrl, 1);
}
function DeleteDataCardItem(deleteObj) {

    let confirmBtnText = deleteObj.confirmBtnText || "Yes, delete it!";
    let cancelBtnText = deleteObj.cancelBtnText || "No, cancel!";
    let deleteUrl = deleteObj.deleteUrl || "";

    swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.value) {
            DeleteCardItem(deleteUrl).then(function (ajaxResult) {
                if (ajaxResult.Success) {
                    if (ajaxResult.ReloadDatatable) {
                        InitializeDataCards();
                    }
                    else {
                        if (deleteReturnUrl === "" || deleteReturnUrl === null || deleteReturnUrl === undefined) {
                            location.reload();
                        }
                        else {
                            window.location.href = deleteReturnUrl;
                        }
                    }

                }
                else {
                    Swal.fire("Couldn't delete. Try again later.")
                }
            });

        }
        else if (result.dismiss === swal.DismissReason.cancel) {
        }
    });


}
function DeleteCardItem(url) {
    return $.ajax({
        url: url,
        type: 'POST',
        success: function (res) {
            InitializeDataCards();
        }
    });
}
function loadUpdateAndDetailModalPanel(contentUrl) {
    loadModalPanel(contentUrl, "crudModalPanel", "crudModalPanelBody")
}


