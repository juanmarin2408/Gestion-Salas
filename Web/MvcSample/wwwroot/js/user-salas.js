let salaSeleccionadaId = null;

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
                salaSeleccionadaId = salaId;
                $('#modalSolicitarTitle').text(`Reservar la sala ${sala.numero}`);
                $('#modalSolicitarSala').text(sala.numero);
                $('#modalSolicitarUbicacion').text(sala.ubicacion);
                $('#modalSolicitarEquipos').text(sala.equiposDisponibles);
                $('#salaIdSolicitar').val(salaId);
                $('#tituloUso').val('');
                $('#motivoUso').val('');
                $('#numeroAsistentes').val('');
                $('#fechaInicioUso').val('');
                $('#fechaFinUso').val('');
                $('#tiempoEstimadoEquipo').val('');
                $('input[name="tipoPrestamo"][value="Equipo"]').prop('checked', true);
                actualizarVistaTipo();

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

$(document).on('change', 'input[name="tipoPrestamo"]', function () {
    actualizarVistaTipo();
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
    salaSeleccionadaId = null;
}

function actualizarVistaTipo() {
    const tipo = $('input[name="tipoPrestamo"]:checked').val();
    if (tipo === 'SalaCompleta') {
        $('#seccionSala').removeClass('hidden');
        $('#seccionEquipo').addClass('hidden');
        $('#reservasSalaWrapper').show();
        if (salaSeleccionadaId) {
            cargarReservasSala(salaSeleccionadaId);
        }
    } else {
        $('#seccionSala').addClass('hidden');
        $('#seccionEquipo').removeClass('hidden');
        $('#reservasSalaWrapper').hide();
    }
}

async function cargarReservasSala(salaId) {
    const $lista = $('#reservasSalaList');
    $lista.html('<div class="cargando-reservas">Cargando reservas...</div>');

    try {
        const response = await fetch(`/User/GetReservasSala/${salaId}`);
        const data = await response.json();

        if (!data.success || !data.reservas || data.reservas.length === 0) {
            $lista.html('<div class="sin-reservas">Sin reservas registradas</div>');
            return;
        }

        const elementos = data.reservas.map(reserva => {
            const inicio = new Date(reserva.inicio);
            const fin = new Date(reserva.fin);
            const titulo = reserva.titulo || 'Uso reservado';
            return `
                <div class="reserva-item">
                    <div class="reserva-horario">
                        <span>${inicio.toLocaleDateString()} · ${inicio.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                        <span class="reserva-arrow">→</span>
                        <span>${fin.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                    </div>
                    <div class="reserva-detalle">
                        <strong>${titulo}</strong>
                        <span>${reserva.estado}</span>
                    </div>
                </div>`;
        }).join('');

        $lista.html(elementos);
    } catch (error) {
        console.error('Error cargando reservas:', error);
        $lista.html('<div class="sin-reservas">No se pudieron cargar las reservas</div>');
    }
}

async function enviarSolicitud(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    const salaId = formData.get('SalaId');
    const tipo = $('input[name="tipoPrestamo"]:checked').val();
    const token = $('input[name="__RequestVerificationToken"]').val();

    const body = new URLSearchParams();
    body.append('SalaId', salaId);
    body.append('Tipo', tipo);
    body.append('__RequestVerificationToken', token);

    if (tipo === 'SalaCompleta') {
        const fechaInicio = formData.get('FechaInicioUso');
        const fechaFin = formData.get('FechaFinUso');
        const motivoUso = (formData.get('MotivoUso') || '').trim();
        const numeroAsistentes = formData.get('NumeroAsistentes');
        const tituloUso = formData.get('TituloUso') || '';

        if (!fechaInicio || !fechaFin) {
            showMessage('Selecciona la fecha y hora de inicio y fin', 'error');
            return;
        }

        if (new Date(fechaFin) <= new Date(fechaInicio)) {
            showMessage('La hora de finalización debe ser mayor a la de inicio', 'error');
            return;
        }

        if (!motivoUso) {
            showMessage('Describe el motivo del préstamo de la sala', 'error');
            return;
        }

        body.append('FechaInicioUso', fechaInicio);
        body.append('FechaFinUso', fechaFin);
        body.append('MotivoUso', motivoUso);
        body.append('NumeroAsistentes', numeroAsistentes || '');
        body.append('TituloUso', tituloUso);
    } else {
        const tiempoEstimado = $('#tiempoEstimadoEquipo').val();
        if (!tiempoEstimado) {
            showMessage('Selecciona el tiempo estimado de uso', 'error');
            return;
        }
        body.append('TiempoEstimado', tiempoEstimado);
    }

    try {
        const response = await fetch('/User/SolicitarPrestamo', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: body.toString()
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

