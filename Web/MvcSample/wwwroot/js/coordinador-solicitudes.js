$(document).ready(function () {
    let currentSolicitudId = null;

    // Aprobar solicitud
    $(document).on('click', 'table .btn-aprobar', async function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        const solicitudId = $(this).attr('data-solicitud-id') || $(this).data('solicitud-id');
        
        if (!solicitudId) {
            console.error('No se pudo obtener el ID de la solicitud para aprobar');
            showMessage('Error: No se pudo identificar la solicitud', 'error');
            return;
        }
        
        if (!confirm('¿Estás seguro de que deseas aprobar esta solicitud?')) {
            return;
        }

        try {
            const token = $('input[name="__RequestVerificationToken"]').val();
            const response = await fetch('/Coordinador/AprobarSolicitud', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: `id=${solicitudId}&__RequestVerificationToken=${token}`
            });

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                
                // Actualizar la fila de la tabla
                const solicitudRow = $(`tr[data-solicitud-id="${solicitudId}"]`);
                if (solicitudRow.length > 0) {
                    solicitudRow.attr('data-estado', '1'); // 1 = Aprobada
                    solicitudRow.find('.status-badge')
                        .removeClass('status-pendiente status-rechazada')
                        .addClass('status-aprobada')
                        .text('Aprobada');
                    
                    // Remover los botones de acción
                    solicitudRow.find('.actions-cell').html('');
                }
                
                setTimeout(() => {
                    location.reload();
                }, 1500);
            } else {
                showMessage(data.message, 'error');
            }
        } catch (error) {
            console.error('Error al aprobar solicitud:', error);
            showMessage('Ocurrió un error al aprobar la solicitud', 'error');
        }
    });

    // Abrir modal de rechazar
    $(document).on('click', 'table .btn-rechazar', function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        // Obtener el ID del atributo data directamente
        const solicitudId = $(this).attr('data-solicitud-id');
        
        if (!solicitudId) {
            console.error('No se pudo obtener el ID de la solicitud');
            console.log('Elemento clickeado:', this);
            console.log('Data attribute:', $(this).attr('data-solicitud-id'));
            return;
        }

        currentSolicitudId = solicitudId;
        const usuarioNombre = $(this).closest('tr').find('.solicitud-user-name').text();
        $('#rechazarSubtitle').text(`Ingresa el motivo del rechazo para ${usuarioNombre}`);
        $('#rechazarSolicitudId').val(solicitudId);
        $('#MotivoRechazo').val('');
        $('#modalMessageRechazar').hide().text('');
        $('#btnRechazar').prop('disabled', false).text('Rechazar Solicitud');
        $('#rechazarModal').fadeIn(300);
        setTimeout(() => {
            $('#MotivoRechazo').focus();
        }, 100);
    });

    // Cerrar modal de rechazar
    $('#closeRechazarModal, #btnCancelRechazar').off('click').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#rechazarModal').fadeOut(300);
        $('#MotivoRechazo').val('');
        $('#rechazarSolicitudId').val('');
        $('#modalMessageRechazar').hide().text('');
        $('#btnRechazar').prop('disabled', false).text('Rechazar Solicitud');
    });

    // Cerrar modal al hacer clic fuera
    $('#rechazarModal').off('click').on('click', function (e) {
        if ($(e.target).is('#rechazarModal')) {
            $(this).fadeOut(300);
            $('#MotivoRechazo').val('');
            $('#rechazarSolicitudId').val('');
            $('#modalMessageRechazar').hide().text('');
            $('#btnRechazar').prop('disabled', false).text('Rechazar Solicitud');
        }
    });

    // Enviar formulario de rechazar
    $('#rechazarForm').off('submit').on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        const motivoRechazo = $('#MotivoRechazo').val().trim();
        const solicitudId = $('#rechazarSolicitudId').val();

        // Validación client-side
        if (!motivoRechazo) {
            showMessage('Por favor ingresa el motivo del rechazo', 'error', '#modalMessageRechazar');
            $('#MotivoRechazo').focus();
            return false;
        }

        if (!solicitudId) {
            showMessage('Error: No se pudo identificar la solicitud', 'error', '#modalMessageRechazar');
            return false;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();
        if (!token) {
            showMessage('Error de seguridad: Token no encontrado', 'error', '#modalMessageRechazar');
            return false;
        }

        $('#btnRechazar').prop('disabled', true).text('Rechazando...');
        $('#modalMessageRechazar').hide();

        try {
            const formData = new URLSearchParams();
            formData.append('id', solicitudId);
            formData.append('MotivoRechazo', motivoRechazo);
            formData.append('__RequestVerificationToken', token);

            const response = await fetch('/Coordinador/RechazarSolicitud', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: formData.toString()
            });

            const data = await response.json();

            if (data.success) {
                showMessage(data.message, 'success');
                $('#rechazarModal').fadeOut(300);
                
                // Actualizar la fila de la tabla o ocultarla
                const solicitudRow = $(`tr[data-solicitud-id="${solicitudId}"]`);
                if (solicitudRow.length > 0) {
                    // Actualizar el estado en la fila
                    solicitudRow.attr('data-estado', '2'); // 2 = Rechazada
                    solicitudRow.find('.status-badge')
                        .removeClass('status-pendiente status-aprobada')
                        .addClass('status-rechazada')
                        .text('Rechazada');
                    
                    // Remover los botones de acción
                    solicitudRow.find('.actions-cell').html('');
                    
                    // Si el filtro está activo y solo muestra pendientes, ocultar la fila
                    const estadoFilter = $('#filterEstado').val();
                    if (estadoFilter === '0' || estadoFilter === '') {
                        // Ocultar la solicitud rechazada si estamos filtrando por pendientes
                        if (estadoFilter === '0') {
                            solicitudRow.fadeOut(300, function() {
                                $(this).remove();
                            });
                        }
                    }
                }
                
                // Recargar después de un breve delay para asegurar que todo se actualice
                setTimeout(() => {
                    location.reload();
                }, 1500);
            } else {
                showMessage(data.message, 'error', '#modalMessageRechazar');
                $('#btnRechazar').prop('disabled', false).text('Rechazar Solicitud');
            }
        } catch (error) {
            console.error('Error al rechazar solicitud:', error);
            showMessage('Ocurrió un error al rechazar la solicitud. Por favor intenta de nuevo.', 'error', '#modalMessageRechazar');
            $('#btnRechazar').prop('disabled', false).text('Rechazar Solicitud');
        }

        return false;
    });

    // Prevenir reset del formulario
    $('#rechazarForm').off('reset').on('reset', function(e) {
        e.preventDefault();
        e.stopPropagation();
        return false;
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

