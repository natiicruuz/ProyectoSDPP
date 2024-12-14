document.addEventListener("DOMContentLoaded", () => {
    // Elementos del DOM
    const reservationsTable = document.getElementById("reservationsTable").querySelector("tbody");
    const menuTable = document.getElementById("menuTable").querySelector("tbody");
    const reservationForm = document.getElementById("reservationForm");
    const menuForm = document.getElementById("menuForm");
    const menuSelect = document.getElementById("menu");

    const reservationsSection = document.getElementById("reservationsSection");
    const menuSection = document.getElementById("menuSection");
    const showMenuFormBtn = document.getElementById("showMenuForm");
    const showReservationFormBtn = document.getElementById("showReservationForm");

    // URLs de la API
    const API_BASE_URL = "http://localhost:7095/api/Reservation";
    const API_MENU_URL = "http://localhost:7095/api/menu";

    // Variables para edición
    let editReservationId = null;
    let editMenuId = null;

    // Alternar entre secciones
    showMenuFormBtn.addEventListener("click", () => {
        reservationsSection.classList.add("hidden");
        menuSection.classList.remove("hidden");
    });

    showReservationFormBtn.addEventListener("click", () => {
        menuSection.classList.add("hidden");
        reservationsSection.classList.remove("hidden");
    });

    // Cargar y renderizar menús
    async function loadMenus() {
        try {
            console.log("Cargando menús...");
            const response = await fetch(API_MENU_URL);
            if (!response.ok) throw new Error(`Error al obtener los menús: ${response.status}`);
            const menus = await response.json();

            menuSelect.innerHTML = menus.map(menu => `<option value="${menu.id}">${menu.name}</option>`).join("");
            menuTable.innerHTML = menus.length
                ? menus.map(renderMenuRow).join("")
                : `<tr><td colspan="5">No hay menús disponibles</td></tr>`;
            attachMenuEvents();
        } catch (error) {
            console.error("Error al cargar los menús:", error);
            menuTable.innerHTML = `<tr><td colspan="5">Error al cargar los menús</td></tr>`;
        }
    }

    function renderMenuRow(menu) {
        const availabilityText = menu.available ? "Sí" : "No";
        return `
            <tr>
                <td>${menu.name}</td>
                <td>${menu.description}</td>
                <td>$${menu.price.toFixed(2)}</td>
                <td>${availabilityText}</td>
                <td>
                    <button class="edit-menu-btn" data-id="${menu.id}" data-name="${menu.name}" data-description="${menu.description}" data-price="${menu.price}" data-available="${menu.available}">Editar</button>
                    <button class="delete-menu-btn" data-id="${menu.id}">Eliminar</button>
                </td>
            </tr>`;
    }

    // Cargar y renderizar reservas
    async function loadReservations() {
        try {
            console.log("Cargando reservas...");
            const response = await fetch(API_BASE_URL);
            if (!response.ok) throw new Error(`Error al obtener las reservas: ${response.status}`);
            const reservations = await response.json();

            reservationsTable.innerHTML = reservations.length
                ? reservations.map(renderReservationRow).join("")
                : `<tr><td colspan="5">No hay reservas disponibles</td></tr>`;
            attachReservationEvents();
        } catch (error) {
            console.error("Error al cargar las reservas:", error);
            reservationsTable.innerHTML = `<tr><td colspan="5">Error al cargar las reservas</td></tr>`;
        }
    }

    function renderReservationRow(reservation) {
        const formattedDate = new Date(reservation.date).toISOString().split("T")[0];
        const formattedTime = new Date(reservation.date).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
        const statusText = reservation.status ? "Confirmada" : "Pendiente";
        const menuName = reservation.menu?.name || "No disponible";

        return `
            <tr>
                <td>${formattedDate}</td>
                <td>${formattedTime}</td>
                <td>${menuName}</td>
                <td>${statusText}</td>
                <td>
                    <button class="edit-reservation-btn" data-id="${reservation.id}" data-menu-id="${reservation.menuId}" data-date="${formattedDate}" data-time="${formattedTime}" data-status="${reservation.status}">Editar</button>
                    <button class="delete-reservation-btn" data-id="${reservation.id}">Eliminar</button>
                </td>
            </tr>`;
    }

    // Manejo de edición y eliminación
    function attachMenuEvents() {
        document.querySelectorAll(".edit-menu-btn").forEach(button =>
            button.addEventListener("click", e => handleEditMenu(e.target))
        );
        document.querySelectorAll(".delete-menu-btn").forEach(button =>
            button.addEventListener("click", e => handleDeleteMenu(e.target.dataset.id))
        );
    }

    function attachReservationEvents() {
        document.querySelectorAll(".edit-reservation-btn").forEach(button =>
            button.addEventListener("click", e => handleEditReservation(e.target))
        );
        document.querySelectorAll(".delete-reservation-btn").forEach(button =>
            button.addEventListener("click", e => handleDeleteReservation(e.target.dataset.id))
        );
    }

    async function handleEditMenu(button) {
        editMenuId = button.dataset.id;
        document.getElementById("name").value = button.dataset.name;
        document.getElementById("description").value = button.dataset.description;
        document.getElementById("price").value = button.dataset.price;
        document.getElementById("available").value = button.dataset.available;
    }

    async function handleEditReservation(button) {
        editReservationId = button.dataset.id;
        document.getElementById("menu").value = button.dataset.menuId;
        document.getElementById("date").value = button.dataset.date;
        document.getElementById("time").value = button.dataset.time;
        document.getElementById("status").value = button.dataset.status;
    }

    async function handleDeleteMenu(id) {
        if (!confirm("¿Eliminar menú?")) return;
        try {
            await fetch(`${API_MENU_URL}/${id}`, { method: "DELETE" });
            loadMenus();
        } catch (error) {
            console.error("Error al eliminar el menú:", error);
        }
    }

    async function handleDeleteReservation(id) {
        if (!confirm("¿Eliminar reserva?")) return;
        try {
            await fetch(`${API_BASE_URL}/${id}`, { method: "DELETE" });
            loadReservations();
        } catch (error) {
            console.error("Error al eliminar la reserva:", error);
        }
    }

    // Manejo del formulario de menús
    menuForm.addEventListener("submit", async function (event) {
        event.preventDefault();
        const name = document.getElementById("name").value;
        const description = document.getElementById("description").value;
        const price = parseFloat(document.getElementById("price").value);
        const available = document.getElementById("available").value === "true";
        const menuData = { name, description, price, available };

        try {
            const response = editMenuId
                ? await fetch(`${API_MENU_URL}/${editMenuId}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(menuData),
                })
                : await fetch(API_MENU_URL, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(menuData),
                });

            if (!response.ok) throw new Error("Error al guardar el menú");
            editMenuId = null;
            menuForm.reset();
            loadMenus();
        } catch (error) {
            console.error("Error al guardar el menú:", error);
        }
    });

    // Manejo del formulario de reservas
    reservationForm.addEventListener("submit", async function (event) {
        event.preventDefault();
        const menuId = document.getElementById("menu").value;
        const date = document.getElementById("date").value;
        const time = document.getElementById("time").value;
        const status = document.getElementById("status").value === "true";
        const reservationData = { menuId, date: `${date}T${time}:00`, status };

        try {
            const response = editReservationId
                ? await fetch(`${API_BASE_URL}/${editReservationId}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(reservationData),
                })
                : await fetch(API_BASE_URL, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(reservationData),
                });

            if (!response.ok) throw new Error("Error al guardar la reserva");
            editReservationId = null;
            reservationForm.reset();
            loadReservations();
        } catch (error) {
            console.error("Error al guardar la reserva:", error);
        }
    });

    // Inicializar
    loadMenus();
    loadReservations();
});
