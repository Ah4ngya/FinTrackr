document.addEventListener("DOMContentLoaded", function () {
    if (!window.expenseData || window.expenseData.length === 0) {
        console.warn("No expense data found for visualization.");
        return;
    }

    // --- Get user and month info from hidden fields or URL ---
    const userNameInput = document.getElementById("UserName");
    const monthPicker = document.getElementById("selectedMonth");

    const userName = userNameInput ? userNameInput.value : "Ismeretlen";
    const selectedMonth = monthPicker ? monthPicker.value : "";

    const allExpenses = window.expenseData;

    // --- Filter by selected month (if any) ---
    let filteredExpenses = allExpenses;

    if (selectedMonth) {
        const [year, month] = selectedMonth.split("-").map(Number);
        filteredExpenses = allExpenses.filter(e => {
            const d = new Date(e.date);
            return d.getMonth() + 1 === month && d.getFullYear() === year;
        });
    }

    if (filteredExpenses.length === 0) {
        console.warn("No expenses for the selected month.");
        return;
    }

    // --- Calculate category totals ---
    const categoryTotals = {};
    filteredExpenses.forEach(e => {
        const cat = e.expenseType || "Ismeretlen";
        categoryTotals[cat] = (categoryTotals[cat] || 0) + e.expenseValue;
    });

    // --- PIE CHART: by category ---
    const catCanvas = document.getElementById("CategoryChart");
    if (catCanvas) {
        new Chart(catCanvas, {
            type: 'pie',
            data: {
                labels: Object.keys(categoryTotals),
                datasets: [{
                    data: Object.values(categoryTotals),
                    backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4CAF50', '#9C27B0']
                }]
            },
            options: {
                plugins: {
                    title: {
                        display: true,
                        text: `Kiadások kategóriánként – ${selectedMonth || "összes"} (${userName})`
                    }
                }
            }
        });
    }

    // --- BAR CHART: by date ---
    const dateTotals = {};
    filteredExpenses.forEach(e => {
        const d = new Date(e.date).toLocaleDateString('hu-HU', {
            day: '2-digit',
            month: '2-digit'
        });
        dateTotals[d] = (dateTotals[d] || 0) + e.expenseValue;
    });

    const dateCanvas = document.getElementById("DateChart");
    if (dateCanvas) {
        new Chart(dateCanvas, {
            type: 'bar',
            data: {
                labels: Object.keys(dateTotals),
                datasets: [{
                    label: 'Kiadás érték (Ft)',
                    data: Object.values(dateTotals),
                    backgroundColor: '#36A2EB'
                }]
            },
            options: {
                scales: {
                    y: { beginAtZero: true },
                },
                plugins: {
                    title: {
                        display: true,
                        text: `Napi kiadások – ${selectedMonth || "összes"} (${userName})`
                    }
                }
            }
        });
    }

    // --- Animate total value ---
    const totalElem = document.getElementById("totalValue");
    if (totalElem) {
        const totalValue = filteredExpenses.reduce((sum, e) => sum + e.expenseValue, 0);
        const target = totalValue;
        let current = 0;
        const step = target / 60;

        const interval = setInterval(() => {
            current += step;
            if (current >= target) {
                current = target;
                clearInterval(interval);
            }
            totalElem.textContent = current.toLocaleString("hu-HU", { maximumFractionDigits: 0 });
        }, 20);
    }
});
