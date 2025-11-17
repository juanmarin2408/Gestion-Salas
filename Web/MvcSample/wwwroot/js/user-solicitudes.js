$(document).ready(function () {
    // Filtrar por estado
    $('#filterEstado').on('change', function () {
        const estado = $(this).val();
        const url = estado === 'Todos' 
            ? '/User/Solicitudes' 
            : `/User/Solicitudes?estado=${estado}`;
        window.location.href = url;
    });

    // Cancelar solicitud
    $(document).on('click', '.btn-cancelar', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const solicitudId = $(this).data('solicitud-id');
        
        if (!solicitudId) {
            showMessage('Error: No se pudo obtener el ID de la solicitud', 'error');
            return;
        }

        if (!confirm('¿Estás seguro de que deseas cancelar esta solicitud?')) {
            return;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();
        if (!token) {
            showMessage('Error de seguridad: Token no encontrado', 'error');
            return;
        }

        const $btn = $(this);
        const originalText = $btn.text();
        $btn.prop('disabled', true).text('Cancelando...');

        const formData = new URLSearchParams();
        formData.append('id', solicitudId);
        formData.append('__RequestVerificationToken', token);

        fetch('/User/CancelarSolicitud', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: formData.toString()
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showMessage(data.message, 'success');
                
                // Actualizar la fila de la tabla
                const solicitudRow = $(`tr[data-solicitud-id="${solicitudId}"]`);
                if (solicitudRow.length > 0) {
                    // Actualizar el estado en la fila
                    solicitudRow.attr('data-estado', '2'); // 2 = Rechazada
                    solicitudRow.find('.status-badge')
                        .removeClass('status-pendiente status-aprobada')
                        .addClass('status-rechazada')
                        .html('<span class="status-icon">✗</span><span>Rechazada</span>');
                    
                    // Actualizar Equipo Asignado
                    solicitudRow.find('.solicitud-equipo').html('<div class="motivo-rechazo"><strong>Motivo:</strong> Cancelada por el usuario</div>');
                    
                    // Remover el botón de acción
                    solicitudRow.find('.solicitud-acciones').html('');
                    
                    // Si el filtro está activo y solo muestra pendientes, ocultar la fila
                    const estadoFilter = $('#filterEstado').val();
                    if (estadoFilter === 'Pendientes') {
                        solicitudRow.fadeOut(300, function() {
                            $(this).remove();
                            
                            // Verificar si no quedan filas
                            if ($('.solicitudes-table tbody tr').length === 0) {
                                location.reload();
                            }
                        });
                    }
                }
                
                // Recargar después de un breve delay si el filtro está activo
                setTimeout(() => {
                    if ($('#filterEstado').val() !== 'Pendientes') {
                        location.reload();
                    }
                }, 1500);
            } else {
                showMessage(data.message, 'error');
                $btn.prop('disabled', false).text(originalText);
            }
        })
        .catch(error => {
            console.error('Error al cancelar solicitud:', error);
            showMessage('Ocurrió un error al cancelar la solicitud. Por favor intenta de nuevo.', 'error');
            $btn.prop('disabled', false).text(originalText);
        });
    });

    // Función para mostrar mensajes
    function showMessage(message, type) {
        let $container = $('#messageContainer');
        if ($container.length === 0) {
            $container = $('<div id="messageContainer"></div>');
            $('.solicitudes-header').after($container);
        }

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

