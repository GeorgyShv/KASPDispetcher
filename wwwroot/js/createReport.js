document.getElementById('createReportForm1').addEventListener('submit', async function (event) {
    event.preventDefault();

    const reportNumberInput = document.getElementById('reportNumber');
    const errorDiv = document.getElementById('documentNumberError');

    if (isNaN(reportNumberInput.value) || !Number.isInteger(Number(reportNumberInput.value))) {
        errorDiv.style.display = 'block';
        return;
    } else {
        errorDiv.style.display = 'none';
    }

    const formData = new FormData(this);

    //const response = await fetch('/Reports?handler=Create', {
    const response = await fetch('/Reports', {
        method: 'POST',
        body: formData
    });

    if (response.ok) {
        alert('Отчёт успешно создан!');
        location.reload();
    } else {
        alert('Ошибка при создании отчёта.');
    }
});