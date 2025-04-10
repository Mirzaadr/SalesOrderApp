let editingIndex = null;

$(document).ready(function () {
    bindInputEvents();
});

function bindInputEvents() {
    $('#itemsTable tbody tr').on('input', '.quantity, .price', function () {
        updateTotals();
    });
}

function toggleRowMode(row, isEditing) {
    row.querySelectorAll('.view-mode').forEach(el => el.style.display = isEditing ? 'none' : '');
    row.querySelectorAll('.edit-mode').forEach(el => el.style.display = isEditing ? '' : 'none');
}

function editRow(btn) {
    if (editingIndex !== null) return;

    const row = btn.closest('tr');
    toggleRowMode(row, true);
    editingIndex = row.rowIndex;

    const actionCell = row.cells[1];
    actionCell.innerHTML = `
                <button onclick="saveRow(this)"><img src="/assets/disk.png"/></button>
                <button onclick="cancelRow(this)"><img src="/assets/close.png"/></button>
            `;
}

function saveRow(btn) {
    const row = btn.closest('tr');
    const inputs = row.querySelectorAll('.edit-mode');
    inputs.forEach(input => {
        const span = input.previousElementSibling;
        span.textContent = input.value;
    });

    toggleRowMode(row, false);

    const actionCell = row.cells[1];
    actionCell.innerHTML = `
                <button onclick="editRow(this)"><img src="/assets/write.png"/></button>
                <button onclick="deleteRow(this)"><img src="/assets/trash.png"/></button>
            `;

    editingIndex = null;
    recalculateTotal(row);
    updateTotals();
    updateRowNumbers();
}

function cancelRow(btn) {
    const row = btn.closest('tr');
    const inputs = row.querySelectorAll('.edit-mode');

    inputs.forEach(input => {
        const span = input.previousElementSibling;
        input.value = span.textContent;
    });

    toggleRowMode(row, false);

    const actionCell = row.cells[1];
    actionCell.innerHTML = `
                <button onclick="editRow(this)"><img src="/assets/write.png"/></button>
                <button onclick="deleteRow(this)"><img src="/assets/trash.png"/></button>
            `;

    recalculateTotal(row);
    updateTotals();
    editingIndex = null;
}

function deleteRow(btn) {
    const row = btn.closest('tr');
    row.remove();
    editingIndex = null;

    reindexItems();
    updateRowNumbers();
}

function addNewRow() {
    if (editingIndex !== null) return;

    const table = document.getElementById('itemsTable').querySelector('tbody');
    const index = table.rows.length;
    const orderID = document.getElementById('SoOrderId').value;

    const row = document.createElement('tr');
    row.innerHTML = `
                <input type="hidden" name="Items[${index}].SoOrderId" value="${orderID}" />
                <td class="row-number">${index + 1}</td>
                <td>
                    <button onclick="saveRow(this)"><img src="/assets/disk.png"/></button>
                    <button onclick="cancelRow(this)"><img src="/assets/close.png"/></button>
                </td>
                <td>
                    <span class="view-mode" style="display: none;"></span>
                    <input class="form-control edit-mode" name="Items[${index}].ItemName" />
                </td>
                <td>
                    <span class="view-mode" style="display: none;"></span>
                    <input class="form-control edit-mode item-qty" name="Items[${index}].Quantity" type="number" />
                </td>
                <td>
                    <span class="view-mode" style="display: none;"></span>
                    <input class="form-control edit-mode item-price" name="Items[${index}].Price" type="number" step="0.1" />
                </td>
                <td>
                    <span name="Items[${index}].Total" disabled class="item-total bg-light" ></span>
                </td>
            `;

    table.appendChild(row);
    editingIndex = row.rowIndex;

    row.querySelectorAll('.item-qty, .item-price').forEach(input => {
        input.addEventListener('input', () => {
            recalculateTotal(row);
            updateTotals();
        });
    });
}

function updateRowNumbers() {
    const rows = document.querySelectorAll('#itemsTable tbody tr');
    rows.forEach((row, index) => {
        row.querySelector('.row-number').textContent = index + 1;
    });
}

function reindexItems() {
    $('#itemsTable tbody tr').each(function (index) {
        $(this).find('input, select, textarea').each(function () {
            const name = $(this).attr('name');
            if (name) {
                const newName = name.replace(/\[\d+\]/, `[${index}]`);
                $(this).attr('name', newName);
            }
        });
    });
}

function recalculateTotal(row) {
    const qty = parseFloat(row.querySelector('.item-qty').value) || 0;
    const price = parseFloat(row.querySelector('.item-price').value) || 0;
    row.querySelector('.item-total').textContent = (qty * price).toFixed(2);
}

function updateTotals() {
    let totalQty = 0;
    let totalPrice = 0;

    document.querySelectorAll('#itemsTable tbody tr').forEach(row => {
        let qty = parseFloat(row.querySelector(".item-qty")?.value || 0);
        let price = parseFloat(row.querySelector(".item-price")?.value || 0);

        if (!isNaN(qty)) totalQty += qty;
        if (!isNaN(price) && !isNaN(qty)) totalPrice += price * qty;
    });

    document.getElementById("totalQuantity").textContent = totalQty;
    document.getElementById("totalPrice").textContent = totalPrice.toFixed(2);
}

// Call this whenever items change
document.addEventListener("DOMContentLoaded", updateTotals);


document.querySelectorAll('.item-qty, .item-price').forEach(input => {
    input.addEventListener('input', function () {
        const row = input.closest('tr');
        recalculateTotal(row);
        updateTotals();
    });
});