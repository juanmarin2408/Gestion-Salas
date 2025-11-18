$(document).ready(function () {
    // Cambiar campos según tipo de reporte
    $('#TipoReporte').on('change', function () {
        const tipo = $(this).val();
        if (tipo === 'Equipo') {
            $('#fieldEquipo').show();
            $('#fieldSala').hide();
            $('#EquipoId').prop('required', true);
            $('#SalaId').prop('required', false).val('');
        } else if (tipo === 'Sala') {
            $('#fieldEquipo').hide();
            $('#fieldSala').show();
            $('#EquipoId').prop('required', false).val('');
            $('#SalaId').prop('required', true);
        }
    });

    // Filtrar por estado
    $('#filterEstado').on('change', function () {
        const estado = $(this).val();
        const url = estado === 'Todos' 
            ? '/User/Reportes' 
            : `/User/Reportes?estado=${estado}`;
        window.location.href = url;
    });

    // Abrir modal de nuevo reporte
    $('#btnNuevoReporte').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        $('#modalNuevoReporte').fadeIn(300);
        $('#TipoReporte').trigger('change'); // Asegurar que los campos correctos estén visibles
        setTimeout(() => {
            const tipo = $('#TipoReporte').val();
            if (tipo === 'Equipo') {
                $('#EquipoId').focus();
            } else if (tipo === 'Sala') {
                $('#SalaId').focus();
            }
        }, 100);
    });

    // Cerrar modal de nuevo reporte
    $('#closeModalReporte, #btnCancelReporte').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#modalNuevoReporte').fadeOut(300);
        $('#formNuevoReporte')[0].reset();
        $('#modalMessageReporte').hide().text('');
        $('#btnEnviarReporte').prop('disabled', false).text('Enviar Reporte');
    });

    // Cerrar modal al hacer clic fuera
    $('#modalNuevoReporte').on('click', function (e) {
        if ($(e.target).is('#modalNuevoReporte')) {
            $(this).fadeOut(300);
            $('#formNuevoReporte')[0].reset();
            $('#TipoReporte').trigger('change'); // Resetear campos visibles
            $('#modalMessageReporte').hide().text('');
            $('#btnEnviarReporte').prop('disabled', false).text('Enviar Reporte');
        }
    });

    // Enviar formulario de nuevo reporte
    $('#formNuevoReporte').on('submit', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        const tipoReporte = $('#TipoReporte').val();
        const equipoId = $('#EquipoId').val();
        const salaId = $('#SalaId').val();
        const descripcion = $('#Descripcion').val().trim();

        // Validación client-side
        if (!tipoReporte) {
            showMessage('Por favor selecciona el tipo de reporte', 'error', '#modalMessageReporte');
            $('#TipoReporte').focus();
            return false;
        }

        if (tipoReporte === 'Equipo' && !equipoId) {
            showMessage('Por favor selecciona un equipo', 'error', '#modalMessageReporte');
            $('#EquipoId').focus();
            return false;
        }

        if (tipoReporte === 'Sala' && !salaId) {
            showMessage('Por favor selecciona una sala', 'error', '#modalMessageReporte');
            $('#SalaId').focus();
            return false;
        }

        if (!descripcion) {
            showMessage('Por favor ingresa la descripción del problema', 'error', '#modalMessageReporte');
            $('#Descripcion').focus();
            return false;
        }

        if (descripcion.length > 1000) {
            showMessage('La descripción no puede exceder 1000 caracteres', 'error', '#modalMessageReporte');
            $('#Descripcion').focus();
            return false;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();
        if (!token) {
            showMessage('Error de seguridad: Token no encontrado', 'error', '#modalMessageReporte');
            return false;
        }

        $('#btnEnviarReporte').prop('disabled', true).text('Enviando...');
        $('#modalMessageReporte').hide();

        try {
            const formData = new URLSearchParams();
            formData.append('TipoReporte', tipoReporte);
            if (tipoReporte === 'Equipo' && equipoId) {
                formData.append('EquipoId', equipoId);
            }
            if (tipoReporte === 'Sala' && salaId) {
                formData.append('SalaId', salaId);
            }
            formData.append('Descripcion', descripcion);
            formData.append('__RequestVerificationToken', token);

            const response = await fetch('/User/CrearReporte', {
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
                $('#modalNuevoReporte').fadeOut(300);
                $('#formNuevoReporte')[0].reset();
                $('#TipoReporte').trigger('change'); // Resetear campos visibles
                
                // Recargar la página después de un breve delay
                setTimeout(() => {
                    location.reload();
                }, 1500);
            } else {
                showMessage(data.message, 'error', '#modalMessageReporte');
                $('#btnEnviarReporte').prop('disabled', false).text('Enviar Reporte');
            }
        } catch (error) {
            console.error('Error al crear reporte:', error);
            showMessage('Ocurrió un error al crear el reporte. Por favor intenta de nuevo.', 'error', '#modalMessageReporte');
            $('#btnEnviarReporte').prop('disabled', false).text('Enviar Reporte');
        }

        return false;
    });

    // Prevenir reset del formulario
    $('#formNuevoReporte').on('reset', function(e) {
        e.preventDefault();
        e.stopPropagation();
        return false;
    });

    // Función para mostrar mensajes
    function showMessage(message, type, container = '#messageContainer') {
        let $container = $(container);
        if ($container.length === 0) {
            $container = $('<div id="messageContainer"></div>');
            $('.reportes-header').after($container);
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

