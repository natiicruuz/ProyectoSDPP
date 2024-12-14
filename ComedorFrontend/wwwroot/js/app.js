document.addEventListener("DOMContentLoaded", async () => {
    const reservationsTable = document.getElementById("reservationsTable").querySelector("tbody");
    const menuTable = document.getElementById("menuTable").querySelector("tbody");
    const menuSelect = document.getElementById("menu");
    const reservationForm = document.getElementById("reservationForm");
    const menuForm = document.getElementById("menuForm");

    const API_BASE_URL = "https://localhost:7095/api/Reservation";
    const API_MENU_URL = "https://localhost:7095/api/menu";

    let editingReservationId = null; // Para editar reservas
    let editingMenuId = null; // Para editar menús

    async function loadMenus() {
        try {
            const response = await fetch(`${API_MENU_URL}`);
            const result = await response.json();
            const menus = result.data;

            // Cargar menús en el select del formulario de reservas
            menuSelect.innerHTML = menus
                .map((menu) => `<option value="${menu.id}">${menu.name}</option>`)
                .join("");

            // Cargar menús en la tabla
            menuTable.innerHTML = menus
                .map((menu) => `
                    <tr>
                        <td>${menu.name}</td>
                        <td>${menu.description}</td>
                        <td>${menu.price.toFixed(2)}</td>
                        <td>${menu.available ? "Sí" : "No"}</td>
                        <td>
                            <button class="edit-btn" data-id="${menu.id}"><i class="fa-solid fa-pen-to-square"></i> Editar</button>
                            <button class="delete-btn" data-id="${menu.id}"><i class="fa-solid fa-trash"></i> Eliminar</button>
                        </td>
                    </tr>
                `)
                .join("");

            // Agregar eventos a los botones de edición y eliminación de menús
            document.querySelectorAll(".edit-btn").forEach((button) =>
                button.addEventListener("click", (e) => handleEditMenu(e.target.dataset.id))
            );

            document.querySelectorAll(".delete-btn").forEach((button) =>
                button.addEventListener("click", (e) => handleDeleteMenu(e.target.dataset.id))
            );
        } catch (error) {
            console.error("Error al cargar los menús:", error);
        }
    }

    async function loadReservations() {
        try {
            const response = await fetch(`${API_BASE_URL}`);
            const reservations = await response.json();

            if (!reservations || reservations.length === 0) {
                reservationsTable.innerHTML = `<tr><td colspan="5">No hay reservas disponibles</td></tr>`;
                return;
            }

            reservationsTable.innerHTML = reservations
                .map((reservation) => {
                    const formattedDate = new Date(reservation.date).toLocaleDateString();
                    const formattedTime = new Date(reservation.date).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                    });
                    const statusText = reservation.status ? "Confirmada" : "Pendiente";
                    const menuName = reservation.menu?.name || "Menú no disponible";

                    return `
                    <tr>
                        <td>${formattedDate}</td>
                        <td>${formattedTime}</td>
                        <td>${menuName}</td>
                        <td>${statusText}</td>
                        <td>
                            <button class="edit-btn" data-id="${reservation.id}"><i class="fa-solid fa-pen-to-square"></i> Editar</button>
                            <button class="delete-btn" data-id="${reservation.id}"><i class="fa-solid fa-trash"></i> Eliminar</button>
                        </td>
                    </tr>`;
                })
                .join("");

            document.querySelectorAll(".edit-btn").forEach((button) =>
                button.addEventListener("click", (e) => handleEditReservation(e.target.dataset.id))
            );

            document.querySelectorAll(".delete-btn").forEach((button) =>
                button.addEventListener("click", (e) => handleDeleteReservation(e.target.dataset.id))
            );
        } catch (error) {
            console.error("Error al cargar las reservas:", error);
            reservationsTable.innerHTML = `<tr><td colspan="5">Error al cargar las reservas</td></tr>`;
        }
    }

    async function handleAddOrUpdateReservation(event) {
        event.preventDefault();

        const formData = new FormData(reservationForm);
        const reservationData = {
            date: `${formData.get("date")}T${formData.get("time")}:00`, // Combinar fecha y hora
            menuId: formData.get("menu"),
            status: formData.get("status") === "true",
        };

        try {
            const response = editingReservationId
                ? await fetch(`${API_BASE_URL}/${editingReservationId}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(reservationData),
                })
                : await fetch(`${API_BASE_URL}`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(reservationData),
                });

            if (response.ok) {
                alert(editingReservationId ? "Reserva actualizada con éxito" : "Reserva guardada con éxito");
                reservationForm.reset();
                editingReservationId = null;
                loadReservations();
            } else {
                alert("Error al guardar la reserva");
            }
        } catch (error) {
            console.error("Error al guardar la reserva:", error);
            alert("Ha ocurrido un error al intentar guardar la reserva");
        }
    }

    // Manejar el envío del formulario de menús (crear o actualizar)
    async function handleAddOrUpdateMenu(event) {
        event.preventDefault();

        const formData = new FormData(menuForm);
        const menuData = {
            name: formData.get("name"),
            description: formData.get("description"),
            price: parseFloat(formData.get("price")),
            available: formData.get("available") === "true",
        };

        try {
            const response = editingMenuId
                ? await fetch(`${API_MENU_URL}/${editingMenuId}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(menuData),
                })
                : await fetch(`${API_MENU_URL}`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(menuData),
                });

            if (response.ok) {
                alert(editingMenuId ? "Menú actualizado con éxito" : "Menú creado con éxito");
                menuForm.reset();
                editingMenuId = null;
                loadMenus();
            } else {
                alert("Error al guardar el menú");
            }
        } catch (error) {
            console.error("Error al guardar el menú:", error);
            alert("Ha ocurrido un error al intentar guardar el menú");
        }
    }

    async function handleEditReservation(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/${id}`);
            const reservation = await response.json();

            if (reservation) {
                // Cargar datos en el formulario
                const date = new Date(reservation.date);
                reservationForm.date.value = date.toISOString().split("T")[0];
                reservationForm.time.value = date.toISOString().split("T")[1].slice(0, 5);
                reservationForm.menu.value = reservation.menuId;
                reservationForm.status.value = reservation.status;

                // Cambiar el modo de edición
                editingReservationId = id;
                alert(`Editando reserva con ID: ${id}`);
            } else {
                alert("No se pudo cargar la reserva para editar");
            }
        } catch (error) {
            console.error("Error al cargar la reserva para editar:", error);
            alert("Error al cargar la reserva para editar");
        }
    }

    // Manejar la edición de un menú
    async function handleEditMenu(id) {
        try {
            const response = await fetch(`${API_MENU_URL}/${id}`);
            const menu = await response.json();

            if (menu) {
                // Cargar datos en el formulario
                menuForm.name.value = menu.name;
                menuForm.description.value = menu.description;
                menuForm.price.value = menu.price;
                menuForm.available.value = menu.available.toString();

                // Cambiar el modo de edición
                editingMenuId = id;
                alert(`Editando menú con ID: ${id}`);
            } else {
                alert("No se pudo cargar el menú para editar");
            }
        } catch (error) {
            console.error("Error al cargar el menú para editar:", error);
            alert("Error al cargar el menú para editar");
        }
    }

    async function handleDeleteReservation(id) {
        const confirmDelete = confirm("¿Estás seguro de que deseas eliminar esta reserva?");
        if (!confirmDelete) return;

        try {
            const response = await fetch(`${API_BASE_URL}/${id}`, {
                method: "DELETE",
            });

            if (response.ok) {
                alert("Reserva eliminada con éxito");
                loadReservations();
            } else {
                alert("Error al eliminar la reserva");
            }
        } catch (error) {
            console.error("Error al eliminar la reserva:", error);
            alert("Ha ocurrido un error al intentar eliminar la reserva");
        }
    }

    // Manejar la eliminación de un menú
    async function handleDeleteMenu(id) {
        const confirmDelete = confirm("¿Estás seguro de que deseas eliminar este menú?");
        if (!confirmDelete) return;

        try {
            const response = await fetch(`${API_MENU_URL}/${id}`, {
                method: "DELETE",
            });

            if (response.ok) {
                alert("Menú eliminado con éxito");
                loadMenus();
            } else {
                alert("Error al eliminar el menú");
            }
        } catch (error) {
            console.error("Error al eliminar el menú:", error);
            alert("Ha ocurrido un error al intentar eliminar el menú");
        }
    }

    // Inicializar eventos y cargar datos
    reservationForm.addEventListener("submit", handleAddOrUpdateReservation);
    menuForm.addEventListener("submit", handleAddOrUpdateReservation);
    loadMenus();
    loadReservations();
});
