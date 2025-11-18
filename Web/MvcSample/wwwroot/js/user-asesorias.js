$(document).ready(function () {
    // Filtrar por estado
    $('#filterEstado').on('change', function () {
        const estado = $(this).val();
        const url = estado === 'Todos' 
            ? '/User/Asesorias' 
            : `/User/Asesorias?estado=${estado}`;
        window.location.href = url;
    });

    // Abrir modal de nueva asesoría
    $('#btnNuevoAsesoria').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        $('#modalNuevoAsesoria').fadeIn(300);
        setTimeout(() => {
            $('#Descripcion').focus();
        }, 100);
    });

    // Cerrar modal de nueva asesoría
    $('#closeModalAsesoria, #btnCancelAsesoria').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#modalNuevoAsesoria').fadeOut(300);
        $('#formNuevoAsesoria')[0].reset();
        $('#modalMessageAsesoria').hide().text('');
        $('#btnEnviarAsesoria').prop('disabled', false).text('Enviar Solicitud');
    });

    // Cerrar modal al hacer clic fuera
    $('#modalNuevoAsesoria').on('click', function (e) {
        if ($(e.target).is('#modalNuevoAsesoria')) {
            $(this).fadeOut(300);
            $('#formNuevoAsesoria')[0].reset();
            $('#modalMessageAsesoria').hide().text('');
            $('#btnEnviarAsesoria').prop('disabled', false).text('Enviar Solicitud');
        }
    });

    // Enviar formulario de nueva asesoría
    $('#formNuevoAsesoria').on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        const descripcion = $('#Descripcion').val().trim();
        const prioridad = $('#Prioridad').val();

        // Validación client-side
        if (!descripcion) {
            showMessage('Por favor ingresa la descripción del problema', 'error', '#modalMessageAsesoria');
            $('#Descripcion').focus();
            return false;
        }

        if (descripcion.length > 1000) {
            showMessage('La descripción no puede exceder 1000 caracteres', 'error', '#modalMessageAsesoria');
            $('#Descripcion').focus();
            return false;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();
        if (!token) {
            showMessage('Error de seguridad: Token no encontrado', 'error', '#modalMessageAsesoria');
            return false;
        }

        $('#btnEnviarAsesoria').prop('disabled', true).text('Enviando...');
        $('#modalMessageAsesoria').hide();

        try {
            const formData = new URLSearchParams();
            formData.append('Descripcion', descripcion);
            formData.append('Prioridad', prioridad);
            formData.append('__RequestVerificationToken', token);

            const response = await fetch('/User/CrearAsesoria', {
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
                $('#modalNuevoAsesoria').fadeOut(300);
                $('#formNuevoAsesoria')[0].reset();
                
                // Recargar la página después de un breve delay
                setTimeout(() => {
                    location.reload();
                }, 1500);
            } else {
                showMessage(data.message, 'error', '#modalMessageAsesoria');
                $('#btnEnviarAsesoria').prop('disabled', false).text('Enviar Solicitud');
            }
        } catch (error) {
            console.error('Error al crear asesoría:', error);
            showMessage('Ocurrió un error al crear la solicitud de asesoría. Por favor intenta de nuevo.', 'error', '#modalMessageAsesoria');
            $('#btnEnviarAsesoria').prop('disabled', false).text('Enviar Solicitud');
        }

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

