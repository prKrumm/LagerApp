// Write your Javascript code.
$(document).ready(function () {
    $("#target").click(function () {
        var artikelNr = $('#artikelnr').val();
        $.get("/lager?artikel=" + artikelNr)
            .done(function (data) {
            });
    });
});


