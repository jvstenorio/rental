// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

$(function () {
    
    $('#partialBooking').on('change', '.updateevent', function (el) {
        $.ajax({
            url: window.location.origin + "/Bookings/Update",
            type: "post",
            data: $("#bookingForm").serialize(),
            success: function (result) {
                $("#partialBooking").html(result);
            },
            error: function (error)
            {
                console.log(error);
            }
        });
    });
});
