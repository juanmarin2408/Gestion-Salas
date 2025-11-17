$(document).ready(function () {
    // Ver detalles de sala
    $(document).on('click', '.btn-ver-detalles', async function () {
        const salaId = $(this).data('sala-id');
        
        try {
            const response = await fetch(`/User/GetSala/${salaId}`);
            const data = await response.json();

            if (data.success) {
                const sala = data.sala;
                $('#modalDetallesTitle').text(`Detalles de ${sala.numero}`);
                $('#modalDetallesUbicacion').text(sala.ubicacion);
                $('#modalDetallesTotalEquipos').text(sala.totalEquipos);
                
                const estadoTexto = sala.equiposDisponibles > 0 ? 'Libre' : 
                                   sala.equiposAsignados > 0 ? 'Ocupada' : 'Mantenimiento';
                $('#modalDetallesEstado').text(estadoTexto);
                $('#modalDetallesCapacidad').text(`${sala.capacidad} personas`);
                $('#modalDetallesEquiposDisponibles').text(sala.equiposDisponibles);
                $('#modalDetallesAsignacion').text(sala.usuarioNombre || 'Sin asignación');

                abrirModalDetalles();
            } else {
                showMessage(data.message, 'error');
            }
        } catch (error) {
            showMessage('Ocurrió un error al obtener los detalles de la sala', 'error');
        }
    });

    // Abrir modal solicitar
    $(document).on('click', '.btn-solicitar:not(.disabled)', async function () {
        const salaId = $(this).data('sala-id');
        const salaCard = $(this).closest('.sala-card');
        const salaNombre = salaCard.find('.sala-nombre').text();
        const salaUbicacion = salaCard.find('.sala-info-item').first().text().trim();
        const equiposDisponibles = salaCard.find('.sala-info-item').last().text().trim().split(' ')[0];

        try {
            const response = await fetch(`/User/GetSala/${salaId}`);
            const data = await response.json();

            if (data.success) {
                const sala = data.sala;
                $('#modalSolicitarTitle').text(`Solicitar Equipo en ${sala.numero}`);
                $('#modalSolicitarSala').text(sala.numero);
                $('#modalSolicitarUbicacion').text(sala.ubicacion);
                $('#modalSolicitarEquipos').text(sala.equiposDisponibles);
                $('#salaIdSolicitar').val(salaId);
                $('#tiempoEstimado').val('');

                abrirModalSolicitar();
            } else {
                showMessage(data.message, 'error');
            }
        } catch (error) {
            showMessage('Ocurrió un error al obtener la información de la sala', 'error');
        }
    });

    // Cerrar modal al hacer clic fuera
    $(window).on('click', function (event) {
        const modalDetalles = $('#modalDetalles');
        const modalSolicitar = $('#modalSolicitar');
        
        if ($(event.target).is(modalDetalles)) {
            cerrarModalDetalles();
        }
        if ($(event.target).is(modalSolicitar)) {
            cerrarModalSolicitar();
        }
    });
});

function abrirModalDetalles() {
    $('#modalDetalles').addClass('show');
    $('body').css('overflow', 'hidden');
}

function cerrarModalDetalles() {
    $('#modalDetalles').removeClass('show');
    $('body').css('overflow', 'auto');
}

function abrirModalSolicitar() {
    $('#modalSolicitar').addClass('show');
    $('body').css('overflow', 'hidden');
}

function cerrarModalSolicitar() {
    $('#modalSolicitar').removeClass('show');
    $('#formSolicitar')[0].reset();
    $('body').css('overflow', 'auto');
}

async function enviarSolicitud(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    const salaId = formData.get('SalaId');
    const tiempoEstimado = formData.get('TiempoEstimado');

    if (!tiempoEstimado || tiempoEstimado <= 0) {
        showMessage('Por favor selecciona un tiempo estimado válido', 'error');
        return;
    }

    try {
        const token = $('input[name="__RequestVerificationToken"]').val();
        const response = await fetch('/User/SolicitarEquipo', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: `SalaId=${salaId}&TiempoEstimado=${tiempoEstimado}&__RequestVerificationToken=${token}`
        });

        const data = await response.json();

        if (data.success) {
            showMessage(data.message, 'success');
            cerrarModalSolicitar();
            setTimeout(() => {
                location.reload();
            }, 1500);
        } else {
            showMessage(data.message, 'error');
        }
    } catch (error) {
        showMessage('Ocurrió un error al enviar la solicitud', 'error');
    }
}

function showMessage(message, type) {
    // Crear contenedor de mensaje si no existe
    let $container = $('#messageContainer');
    if ($container.length === 0) {
        $container = $('<div id="messageContainer" style="position: fixed; top: 20px; right: 20px; z-index: 9999; padding: 16px 24px; border-radius: 8px; font-weight: 600; box-shadow: 0 4px 12px rgba(0,0,0,0.15);"></div>');
        $('body').append($container);
    }

    $container.removeClass('message-success message-error')
        .addClass(type === 'success' ? 'message-success' : 'message-error')
        .css({
            'background-color': type === 'success' ? '#d1fae5' : '#fee2e2',
            'color': type === 'success' ? '#065f46' : '#dc2626'
        })
        .text(message)
        .fadeIn(300);

    if (type === 'success') {
        setTimeout(() => {
            $container.fadeOut(300);
        }, 3000);
    }
}

