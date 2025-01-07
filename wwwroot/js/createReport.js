document.getElementById('createReportForm').addEventListener('submit', async function (event) {
    event.preventDefault(); // Предотвращаем стандартную отправку формы

    const formData = new FormData(this);

    try {
        const response = await fetch('/Reports?handler=Create', {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                // Закрываем модальное окно
                const modal = bootstrap.Modal.getInstance(document.getElementById('createReportModal'));
                modal.hide();

                // Обновляем таблицу
                await refreshReportsTable();
            } else {
                alert('Ошибка: ' + (result.error || 'Неизвестная ошибка.'));
            }
        } else {
            const errorText = await response.text();
            console.error('Ошибка сервера:', errorText);
        }
    } catch (error) {
        console.error('Ошибка выполнения:', error);
    }
});

async function refreshReportsTable() {
    try {
        const response = await fetch('/Reports');
        if (response.ok) {
            const html = await response.text();
            const tempDiv = document.createElement('div');
            tempDiv.innerHTML = html;

            // Заменяем содержимое таблицы
            const newTable = tempDiv.querySelector('#reportsTable');
            const oldTable = document.getElementById('reportsTable');
            if (newTable && oldTable) {
                oldTable.innerHTML = newTable.innerHTML;
            }
        } else {
            console.error('Не удалось обновить таблицу отчётов.');
        }
    } catch (error) {
        console.error('Ошибка при обновлении таблицы:', error);
    }
}
