// Elementos del DOM
const form = document.getElementById('reservationForm');

// URL base de la API
const API_BASE_URL = 'https://localhost:7095/api/Reservation';
const API_MENU_URL = 'https://localhost:7095/api/menu';


document.addEventListener("DOMContentLoaded", async () => {
    const reservationsTable = document.getElementById('reservationsTable').querySelector('tbody');
    const menuSelect = document.getElementById("menu");
    // Cargar menus desde la base de datos

    console.log('menuSelect', menuSelect)
    async function loadMenus() {
        try {
            const response = await fetch(`${API_MENU_URL}`);
            const result = await response.json(); // Obtener el objeto completo
            const menus = result.data; // Acceder al array de menús

            menuSelect.innerHTML = menus
                .map((menu) => `<option value="${menu.id}">${menu.name}</option>`)
                .join("");
        } catch (error) {
            console.error("Error al cargar los menús:", error);
        }
    }

    // Cargar reservas desde la base de datos
    async function loadReservations() {
        try {
            const response = await fetch(`${API_BASE_URL}`);
            const reservations = await response.json();

            if (!reservations || reservations.length === 0) {
                reservationsTable.innerHTML = `<tr><td colspan="5">No hay reservas disponibles</td></tr>`;
                return;
            }
            console.log('reservations', reservations)
            reservationsTable.innerHTML = reservations
                .map((reservation) => {
                    const formattedDate = new Date(reservation.date).toLocaleDateString();
                    const formattedTime = new Date(reservation.date).toLocaleTimeString([], {
                        hour: '2-digit',
                        minute: '2-digit',
                    });
                    const statusText = reservation.status ? "Confirmada" : "Pendiente";
                    // Asegúrate de acceder al `menu.name` correctamente
                    const menuName = reservation.menu?.name || "Menú no disponible";

                    return `
                    <tr>
                        <td>${formattedDate}</td>
                        <td>${formattedTime}</td>
                        <td>${menuName}</td>
                        <td>${statusText}</td>
                    </tr>`;
                })
                .join("");
        } catch (error) {
            console.error("Error al cargar las reservas:", error);
            reservationsTable.innerHTML = `<tr><td colspan="5">Error al cargar las reservas</td></tr>`;
        }
    }

    // Crear una nueva reserva
    form.addEventListener('submit', async function (event) {
        event.preventDefault(); // Evita el comportamiento predeterminado del formulario

        // Captura los datos de los inputs
        const menuId = document.getElementById('menu').value;
        const date = document.getElementById('date').value;
        const time = document.getElementById('time').value;
        const status = document.getElementById('status').checked;

        console.log('menuId', menuId)
        // Combina date y time en un formato válido
        const combinedDateTime = `${date}T${time}:00`;

        try {
            // Construye el body para la API
            const reservationData = {
                menuId,
                date: combinedDateTime,
                status,
            };
            console.log('reservationData', reservationData)


            // Llama al API para crear la reserva
            const response = await fetch(`${API_BASE_URL}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(reservationData),
            });

            // Maneja la respuesta de la API
            if (response.ok) {
                alert('Reserva creada con éxito');
                form.reset(); // Limpia el formulario
                loadReservations();
            } else {
                const error = await response.json();
                alert(`Error al crear la reserva: ${error.message}`);
            }
        } catch (error) {
            console.error('Error al conectar con la API:', error);
            alert('Ha ocurrido un error. Por favor, inténtalo nuevamente.');
        }
    });

    // Cargar los datos iniciales
    loadMenus()
    loadReservations();

})
