$(document).ready(function () {
    // Obtener configuración desde window o desde data attributes
    let fechaSemana = (window.calendarioConfig && window.calendarioConfig.fechaSemana) 
        || $('#semanaInfo').data('fecha-semana') 
        || '';
    let salaFiltro = (window.calendarioConfig && window.calendarioConfig.salaFiltro)
        || $('#filterSala').data('sala-filtro')
        || '';

    // Inicializar el filtro de sala si hay uno seleccionado
    if (salaFiltro) {
        $('#filterSala').val(salaFiltro);
    }

    // Navegación de semanas
    $('#btnAnteriorSemana').on('click', function () {
        const partes = fechaSemana.split('-');
        const fecha = new Date(parseInt(partes[0]), parseInt(partes[1]) - 1, parseInt(partes[2]));
        fecha.setDate(fecha.getDate() - 7);
        const año = fecha.getFullYear();
        const mes = String(fecha.getMonth() + 1).padStart(2, '0');
        const dia = String(fecha.getDate()).padStart(2, '0');
        fechaSemana = año + '-' + mes + '-' + dia;
        cargarCalendario();
    });

    $('#btnSiguienteSemana').on('click', function () {
        const partes = fechaSemana.split('-');
        const fecha = new Date(parseInt(partes[0]), parseInt(partes[1]) - 1, parseInt(partes[2]));
        fecha.setDate(fecha.getDate() + 7);
        const año = fecha.getFullYear();
        const mes = String(fecha.getMonth() + 1).padStart(2, '0');
        const dia = String(fecha.getDate()).padStart(2, '0');
        fechaSemana = año + '-' + mes + '-' + dia;
        cargarCalendario();
    });

    $('#btnHoy').on('click', function () {
        const hoy = new Date();
        const diaSemana = hoy.getDay() === 0 ? 7 : hoy.getDay();
        const lunes = new Date(hoy);
        lunes.setDate(hoy.getDate() - (diaSemana - 1));
        const año = lunes.getFullYear();
        const mes = String(lunes.getMonth() + 1).padStart(2, '0');
        const dia = String(lunes.getDate()).padStart(2, '0');
        fechaSemana = año + '-' + mes + '-' + dia;
        cargarCalendario();
    });

    // Filtro por sala
    $('#filterSala').on('change', function () {
        salaFiltro = $(this).val();
        if (!salaFiltro) {
            // Si no hay sala seleccionada, mantener la primera
            salaFiltro = $('#filterSala option:not([value=""])').first().val();
            $(this).val(salaFiltro);
        }
        cargarCalendario();
    });

    function cargarCalendario() {
        const url = new URL('/Coordinador/Calendario', window.location.origin);
        url.searchParams.append('fechaSemana', fechaSemana);
        if (salaFiltro) {
            url.searchParams.append('salaId', salaFiltro);
        }

        window.location.href = url.toString();
    }

    // Formatear fecha para mostrar
    function formatearSemana(fecha) {
        const fechaObj = new Date(fecha);
        const lunes = new Date(fechaObj);
        const viernes = new Date(fechaObj);
        viernes.setDate(lunes.getDate() + 4);
        
        const opciones = { day: '2-digit', month: '2-digit', year: 'numeric' };
        return lunes.toLocaleDateString('es-ES', opciones) + ' - ' + viernes.toLocaleDateString('es-ES', opciones);
    }
});

