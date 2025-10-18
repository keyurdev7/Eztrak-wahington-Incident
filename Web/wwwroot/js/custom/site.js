// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).off("click", '.image-thumbnail')
$(document).on("click", '.image-thumbnail', function () {
    $(".fullsize").attr("src", $(this).attr("src"));
    $('.overlay').fadeIn();
    $('.fullsize').fadeIn();
});

$(document).off("click", '.overlay')
$(document).on("click", '.overlay', function () {
    $('.overlay').fadeOut();
    $('.fullsize').fadeOut();
});
