
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

// Filtriranje hotela
document.addEventListener("DOMContentLoaded", function () {
    console.log("Accommodation filter loaded");

    const checkboxes = document.querySelectorAll(".amenity-filter");
    const hotelCards = document.querySelectorAll(".hotel-card");

    console.log("Checkboxes:", checkboxes.length);
    console.log("Hotel cards:", hotelCards.length);

    function filterHotels() {
        const selectedAmenities = Array.from(checkboxes)
            .filter(checkbox => checkbox.checked)
            .map(checkbox => checkbox.value);

        console.log("Selected:", selectedAmenities);

        hotelCards.forEach(card => {
            const hotelAmenities = card.dataset.amenities || "";

            const shouldShow = selectedAmenities.every(amenity =>
                hotelAmenities.includes(amenity)
            );

            card.style.display = shouldShow ? "" : "none";
        });
    }

    checkboxes.forEach(checkbox => {
        checkbox.addEventListener("change", filterHotels);
    });
});