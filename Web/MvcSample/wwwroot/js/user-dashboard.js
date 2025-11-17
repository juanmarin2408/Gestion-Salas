$(document).ready(function () {
    // Liberar equipo
    $(document).on('click', '.user-btn-liberar', async function () {
        const equipoId = $(this).data('equipo-id');
        const equipoCodigo = $(this).closest('.user-equipo-card').find('.user-equipo-codigo').text();
        
        if (!confirm(`¿Estás seguro de que deseas liberar el equipo ${equipoCodigo}?`)) {
            return;
        }

        try {
            const response = await fetch('/User/LiberarEquipo', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: `id=${equipoId}&__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
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
            showMessage('Ocurrió un error al liberar el equipo', 'error');
        }
    });

    // Solicitar equipo
    $(document).on('click', '.user-btn-solicitar', function () {
        const salaId = $(this).data('sala-id');
        // TODO: Implementar modal para solicitar equipo
        alert('Funcionalidad de solicitar equipo próximamente');
    });

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
});

