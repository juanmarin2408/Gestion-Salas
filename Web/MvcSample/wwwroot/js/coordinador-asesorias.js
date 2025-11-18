$(document).ready(function () {
    let currentAsesoriaId = null;

    // Filtrar por estado
    $('#filterEstado').on('change', function () {
        const estado = $(this).val();
        const url = estado === 'Todos' 
            ? '/Coordinador/Asesorias' 
            : `/Coordinador/Asesorias?estado=${estado}`;
        window.location.href = url;
    });

    // Aceptar asesoría
    $(document).on('click', '.btn-aceptar', async function () {
        const asesoriaId = $(this).data('asesoria-id');
        const confirmacion = confirm('¿Estás seguro de que deseas aceptar esta asesoría? Se marcará como "En Proceso".');

        if (!confirmacion) return;

        const token = $('input[name="__RequestVerificationToken"]').val();
        if (!token) {
            showMessage('Error de seguridad: Token no encontrado', 'error');
            return;
        }

        $(this).prop('disabled', true).text('Aceptando...');

        try {
            const formData = new URLSearchParams();
            formData.append('id', asesoriaId);
            formData.append('__RequestVerificationToken', token);

            const response = await fetch('/Coordinador/AceptarAsesoria', {
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
                setTimeout(() => {
                    location.reload();
                }, 1500);
            } else {
                showMessage(data.message, 'error');
                $(this).prop('disabled', false).text('Aceptar');
            }
        } catch (error) {
            console.error('Error al aceptar asesoría:', error);
            showMessage('Ocurrió un error al aceptar la asesoría. Por favor intenta de nuevo.', 'error');
            $(this).prop('disabled', false).text('Aceptar');
        }
    });

    // Abrir modal de rechazar
    $(document).on('click', '.btn-rechazar-asesoria', function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        const asesoriaId = $(this).data('asesoria-id');
        
        if (!asesoriaId) {
            console.error('No se pudo obtener el ID de la asesoría');
            return;
        }

        currentAsesoriaId = asesoriaId;
        $('#rechazarAsesoriaId').val(asesoriaId);
        $('#MotivoRechazo').val('');
        $('#modalMessageRechazar').hide().text('');
        $('#btnRechazar').prop('disabled', false).text('Rechazar Asesoría');
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
        $('#rechazarAsesoriaId').val('');
        $('#modalMessageRechazar').hide().text('');
        $('#btnRechazar').prop('disabled', false).text('Rechazar Asesoría');
    });

    // Cerrar modal al hacer clic fuera
    $('#rechazarModal').off('click').on('click', function (e) {
        if ($(e.target).is('#rechazarModal')) {
            $(this).fadeOut(300);
            $('#MotivoRechazo').val('');
            $('#rechazarAsesoriaId').val('');
            $('#modalMessageRechazar').hide().text('');
            $('#btnRechazar').prop('disabled', false).text('Rechazar Asesoría');
        }
    });

    // Enviar formulario de rechazar
    $('#rechazarForm').off('submit').on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        const motivoRechazo = $('#MotivoRechazo').val().trim();
        const asesoriaId = $('#rechazarAsesoriaId').val();

        // Validación client-side
        if (!motivoRechazo) {
            showMessage('Por favor ingresa el motivo del rechazo', 'error', '#modalMessageRechazar');
            $('#MotivoRechazo').focus();
            return false;
        }

        if (!asesoriaId) {
            showMessage('Error: No se pudo identificar la asesoría', 'error', '#modalMessageRechazar');
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
            formData.append('id', asesoriaId);
            formData.append('MotivoRechazo', motivoRechazo);
            formData.append('__RequestVerificationToken', token);

            const response = await fetch('/Coordinador/RechazarAsesoria', {
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
                
                // Recargar después de un breve delay
                setTimeout(() => {
                    location.reload();
                }, 1500);
            } else {
                showMessage(data.message, 'error', '#modalMessageRechazar');
                $('#btnRechazar').prop('disabled', false).text('Rechazar Asesoría');
            }
        } catch (error) {
            console.error('Error al rechazar asesoría:', error);
            showMessage('Ocurrió un error al rechazar la asesoría. Por favor intenta de nuevo.', 'error', '#modalMessageRechazar');
            $('#btnRechazar').prop('disabled', false).text('Rechazar Asesoría');
        }

        return false;
    });

    // Prevenir reset del formulario
    $('#rechazarForm').off('reset').on('reset', function(e) {
        e.preventDefault();
        e.stopPropagation();
        return false;
    });

    // Abrir modal de resolver
    $(document).on('click', '.btn-resolver-asesoria', function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        const asesoriaId = $(this).data('asesoria-id');
        
        if (!asesoriaId) {
            console.error('No se pudo obtener el ID de la asesoría');
            return;
        }

        $('#resolverAsesoriaId').val(asesoriaId);
        $('#Observaciones').val('');
        $('#modalMessageResolver').hide().text('');
        $('#btnResolver').prop('disabled', false).text('Marcar como Completada');
        $('#resolverModal').fadeIn(300);
        setTimeout(() => {
            $('#Observaciones').focus();
        }, 100);
    });

    // Cerrar modal de resolver
    $('#closeResolverModal, #btnCancelResolver').off('click').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#resolverModal').fadeOut(300);
        $('#Observaciones').val('');
        $('#resolverAsesoriaId').val('');
        $('#modalMessageResolver').hide().text('');
        $('#btnResolver').prop('disabled', false).text('Marcar como Completada');
    });

    // Cerrar modal al hacer clic fuera
    $('#resolverModal').off('click').on('click', function (e) {
        if ($(e.target).is('#resolverModal')) {
            $(this).fadeOut(300);
            $('#Observaciones').val('');
            $('#resolverAsesoriaId').val('');
            $('#modalMessageResolver').hide().text('');
            $('#btnResolver').prop('disabled', false).text('Marcar como Completada');
        }
    });

    // Enviar formulario de resolver
    $('#resolverForm').off('submit').on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        const observaciones = $('#Observaciones').val().trim();
        const asesoriaId = $('#resolverAsesoriaId').val();

        if (!asesoriaId) {
            showMessage('Error: No se pudo identificar la asesoría', 'error', '#modalMessageResolver');
            return false;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();
        if (!token) {
            showMessage('Error de seguridad: Token no encontrado', 'error', '#modalMessageResolver');
            return false;
        }

        $('#btnResolver').prop('disabled', true).text('Marcando como completada...');
        $('#modalMessageResolver').hide();

        try {
            const formData = new URLSearchParams();
            formData.append('id', asesoriaId);
            formData.append('Observaciones', observaciones);
            formData.append('__RequestVerificationToken', token);

            const response = await fetch('/Coordinador/ResolverAsesoria', {
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
                $('#resolverModal').fadeOut(300);
                
                // Recargar después de un breve delay
                setTimeout(() => {
                    location.reload();
                }, 1500);
            } else {
                showMessage(data.message, 'error', '#modalMessageResolver');
                $('#btnResolver').prop('disabled', false).text('Marcar como Completada');
            }
        } catch (error) {
            console.error('Error al resolver asesoría:', error);
            showMessage('Ocurrió un error al resolver la asesoría. Por favor intenta de nuevo.', 'error', '#modalMessageResolver');
            $('#btnResolver').prop('disabled', false).text('Marcar como Completada');
        }

        return false;
    });

    // Prevenir reset del formulario de resolver
    $('#resolverForm').off('reset').on('reset', function(e) {
        e.preventDefault();
        e.stopPropagation();
        return false;
    });

    // Función para mostrar mensajes
    function showMessage(message, type, container = '#messageContainer') {
        let $container = $(container);
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
});

