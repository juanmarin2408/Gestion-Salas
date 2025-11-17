$(document).ready(function () {
    let currentSolicitudId = null;

    // Aprobar solicitud
    $(document).on('click', '.btn-aprobar', async function () {
        const solicitudId = $(this).data('solicitud-id');
        
        if (!confirm('¿Estás seguro de que deseas aprobar esta solicitud?')) {
            return;
        }

        try {
            const response = await fetch('/Coordinador/AprobarSolicitud', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: `id=${solicitudId}&__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
            });

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                setTimeout(() => {
                    location.reload();
                }, 1000);
            } else {
                showMessage(data.message, 'error');
            }
        } catch (error) {
            showMessage('Ocurrió un error al aprobar la solicitud', 'error');
        }
    });

    // Abrir modal de rechazar
    $(document).on('click', '.btn-rechazar', function () {
        currentSolicitudId = $(this).data('solicitud-id');
        const usuarioNombre = $(this).closest('tr').find('.solicitud-user-name').text();
        $('#rechazarSubtitle').text(`Ingresa el motivo del rechazo para ${usuarioNombre}`);
        $('#rechazarSolicitudId').val(currentSolicitudId);
        $('#rechazarForm')[0].reset();
        $('#rechazarModal').fadeIn(300);
    });

    // Cerrar modal de rechazar
    $('#closeRechazarModal, #btnCancelRechazar').on('click', function () {
        $('#rechazarModal').fadeOut(300);
        $('#rechazarForm')[0].reset();
        $('#modalMessageRechazar').hide();
    });

    // Cerrar modal al hacer clic fuera
    $('#rechazarModal').on('click', function (e) {
        if ($(e.target).is('#rechazarModal')) {
            $(this).fadeOut(300);
            $('#rechazarForm')[0].reset();
            $('#modalMessageRechazar').hide();
        }
    });

    // Enviar formulario de rechazar
    $('#rechazarForm').on('submit', async function (e) {
        e.preventDefault();

        const formData = {
            id: $('#rechazarSolicitudId').val(),
            MotivoRechazo: $('#MotivoRechazo').val(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        $('#btnRechazar').prop('disabled', true).text('Rechazando...');

        try {
            const response = await fetch('/Coordinador/RechazarSolicitud', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': formData.__RequestVerificationToken
                },
                body: new URLSearchParams(formData).toString()
            });

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                $('#rechazarModal').fadeOut(300);
                setTimeout(() => {
                    location.reload();
                }, 1000);
            } else {
                showMessage(data.message, 'error', '#modalMessageRechazar');
            }
        } catch (error) {
            showMessage('Ocurrió un error al rechazar la solicitud', 'error', '#modalMessageRechazar');
        } finally {
            $('#btnRechazar').prop('disabled', false).text('Rechazar Solicitud');
        }
    });

    // Búsqueda
    $('#searchInput').on('keyup', function () {
        const searchTerm = $(this).val().toLowerCase();
        filterTable(searchTerm, $('#filterEstado').val());
    });

    // Filtro por estado
    $('#filterEstado').on('change', function () {
        filterTable($('#searchInput').val().toLowerCase(), $(this).val());
    });

    function filterTable(searchTerm, estadoFilter) {
        $('#solicitudesTableBody tr').each(function () {
            const usuario = $(this).find('.solicitud-user-name').text().toLowerCase();
            const email = $(this).find('.solicitud-user-email').text().toLowerCase();
            const sala = $(this).find('td').eq(1).text().toLowerCase();
            const estado = $(this).data('estado')?.toString() || '';

            const matchesSearch = usuario.includes(searchTerm) || email.includes(searchTerm) || sala.includes(searchTerm);
            const matchesEstado = !estadoFilter || estado === estadoFilter;

            $(this).toggle(matchesSearch && matchesEstado);
        });
    }

    function showMessage(message, type, container = '#messageContainer') {
        const $container = $(container);
        $container.removeClass('message-success message-error')
            .addClass(type === 'success' ? 'message-success' : 'message-error')
            .text(message)
            .fadeIn(300);

        if (type === 'success') {
            setTimeout(() => {
                $container.fadeOut(300);
            }, 3000);
        }
    }
});

