// Referencias a botones y secciones
const showReservationFormButton = document.getElementById("showReservationForm");
const showMenuFormButton = document.getElementById("showMenuForm");
const reservationsSection = document.getElementById("reservationsSection");
const menuSection = document.getElementById("menuSection");

// Mostrar sección de reservaciones y ocultar la de menús
showReservationFormButton.addEventListener("click", () => {
    reservationsSection.classList.remove("hidden");
    menuSection.classList.add("hidden");
});

// Mostrar sección de menús y ocultar la de reservaciones
showMenuFormButton.addEventListener("click", () => {
    menuSection.classList.remove("hidden");
    reservationsSection.classList.add("hidden");
});