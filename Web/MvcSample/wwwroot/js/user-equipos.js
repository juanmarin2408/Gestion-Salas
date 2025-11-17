$(document).ready(function () {
    let currentEquipoId = null;
    let currentEquipoCodigo = null;

    // Abrir modal de liberar equipo
    $(document).on('click', '.user-btn-liberar', function (e) {
        e.preventDefault();
        e.stopPropagation();

        currentEquipoId = $(this).data('equipo-id');
        currentEquipoCodigo = $(this).data('equipo-codigo');

        if (!currentEquipoId) {
            console.error('No se pudo obtener el ID del equipo');
            return;
        }

        // Obtener información del equipo desde la tarjeta
        const equipoCard = $(this).closest('.user-equipo-card');
        const tiempoUso = equipoCard.find('.detail-value').eq(2).text(); // Tiempo de uso está en el tercer detail

        // Actualizar el modal
        $('#modalQuestionLiberar').text(`¿Estás seguro de que quieres liberar el equipo ${currentEquipoCodigo}?`);
        $('#modalTiempoUso').text(tiempoUso);
        $('#modalMessageLiberar').hide().text('');
        $('#btnConfirmLiberar').prop('disabled', false).text('Liberar Equipo');

        // Mostrar el modal
        $('#modalLiberarEquipo').fadeIn(300);
    });

    // Cerrar modal
    $('#closeModalLiberar, #btnCancelLiberar').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#modalLiberarEquipo').fadeOut(300);
        currentEquipoId = null;
        currentEquipoCodigo = null;
        $('#modalMessageLiberar').hide().text('');
        $('#btnConfirmLiberar').prop('disabled', false).text('Liberar Equipo');
    });

    // Cerrar modal al hacer clic fuera
    $('#modalLiberarEquipo').on('click', function (e) {
        if ($(e.target).is('#modalLiberarEquipo')) {
            $(this).fadeOut(300);
            currentEquipoId = null;
            currentEquipoCodigo = null;
            $('#modalMessageLiberar').hide().text('');
            $('#btnConfirmLiberar').prop('disabled', false).text('Liberar Equipo');
        }
    });

    // Confirmar liberación
    $('#btnConfirmLiberar').on('click', async function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (!currentEquipoId) {
            showMessage('Error: No se pudo identificar el equipo', 'error', '#modalMessageLiberar');
            return;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();
        if (!token) {
            showMessage('Error de seguridad: Token no encontrado', 'error', '#modalMessageLiberar');
            return;
        }

        $('#btnConfirmLiberar').prop('disabled', true).text('Liberando...');
        $('#modalMessageLiberar').hide();

        try {
            const formData = new URLSearchParams();
            formData.append('id', currentEquipoId);
            formData.append('__RequestVerificationToken', token);

            const response = await fetch('/User/LiberarEquipo', {
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
                $('#modalLiberarEquipo').fadeOut(300);
                setTimeout(() => {
                    location.reload();
                }, 1000);
            } else {
                showMessage(data.message, 'error', '#modalMessageLiberar');
                $('#btnConfirmLiberar').prop('disabled', false).text('Liberar Equipo');
            }
        } catch (error) {
            console.error('Error al liberar equipo:', error);
            showMessage('Ocurrió un error al liberar el equipo. Por favor intenta de nuevo.', 'error', '#modalMessageLiberar');
            $('#btnConfirmLiberar').prop('disabled', false).text('Liberar Equipo');
        }
    });

    // Función para mostrar mensajes
    function showMessage(message, type, container = '#messageContainer') {
        const $container = $(container);

        if ($container.length === 0) {
            // Crear contenedor de mensaje si no existe
            const $newContainer = $('<div id="messageContainer" style="position: fixed; top: 20px; right: 20px; z-index: 9999; padding: 16px 24px; border-radius: 8px; font-weight: 600; box-shadow: 0 4px 12px rgba(0,0,0,0.15);"></div>');
            $('body').append($newContainer);
            return showMessage(message, type, '#messageContainer');
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

