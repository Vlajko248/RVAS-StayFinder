
// Izlistavanje svih hotela
document.addEventListener("DOMContentLoaded", function () {
    const hotelLinks = document.querySelectorAll(".hotel-details-link");

    hotelLinks.forEach(function (link) {
        link.addEventListener("click", function () {
            console.log("Opening hotel details:", link.textContent.trim());
        });
    });
});



// Pretaga hotela
document.addEventListener("DOMContentLoaded", function () {

    const form = document.querySelector(".form-wrap");

    form.addEventListener("submit", function (e) {

        const location =
            document.getElementById("searchLocation").value.trim();

        if (location.length < 2) {

            e.preventDefault();

            alert("Please enter a location.");
        }
    });
});