$(function () {
    // $(document).off("click", "#global_search_clear");
    $(document).on("click", "#global_search_clear", function (e) {
        e.preventDefault();
        $("#Search_value").val("");
        $(".search-form-btn").click();
    });
    // $(document).off("click", "#global_search_submit");
    $(document).on("click", "#global_search_submit", function (e) {
        // debugger;
        e.preventDefault();
        $("#Search_value").val($("#global_search_value").val());
        $(".search-form-btn").click();
    });

    $('#global_search_value').on('keypress', function(e) {
        if (e.which === 13) {
            e.preventDefault();
            $("#Search_value").val($(this).val());
            $(".search-form-btn").click();
        }
    });
});
