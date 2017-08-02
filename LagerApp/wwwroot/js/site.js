// Write your Javascript code.
$(document).ready(function () {
    $("#target").click(function () {
        var artikelNr = $('#artikelnr').val();
        //check Regex
        var regex = new RegExp("[a-cA-CsS][0-9]{5,5}");
        if (regex.test(artikelNr)) {
            $.get("/lager/" + artikelNr)
                .done(function (data) {
                    $('#pickTabelle tr:last').after('<tr><td>leer</td><td>' + artikelNr + '</td><td>' + data.artikelBezeichnung + '</td><td>' + data.lagerBox + '</td><td>' + data.lagerPlatz + '</td></tr > ');




                });
        } else {
            alert(artikelNr + " ist keine gültige ArtikelNr");
        }

       
    });
});


