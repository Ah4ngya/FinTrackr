document.addEventListener("DOMContentLoaded", function () {
    if (!window.expenseData || window.expenseData.length === 0) {
        console.warn("No expense data found for visualization.");
        return;
    }
    

    const allExpenses = window.expenseData;
    const monthInput = document.getElementById("monthSelect");
    let selectedYear, selectedMonth;

    if (monthInput && monthInput.value) {
        [selectedYear, selectedMonth] = monthInput.value.split("-").map(Number);
    } else {
        const now = new Date();
        selectedYear = now.getFullYear();
        selectedMonth = now.getMonth() + 1;
    }

    const expenses = allExpenses.filter(e => {
        const d = new Date(e.date);
        return d.getFullYear() === selectedYear && (d.getMonth() + 1) === selectedMonth;
    });


    if (expenses.length === 0) {
        console.warn("No expenses for this month.");
        return;
    }

    const categoryTotals = {};
    expenses.forEach(e => {
        const cat = e.expenseType || "Ismeretlen";
        categoryTotals[cat] = (categoryTotals[cat] || 0) + e.expenseValue;
    });

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
                        text: 'Kiadások kategóriánként (aktuális hónap)'
                    }
                }
            }
        });
    }

    const dateTotals = {};
    expenses.forEach(e => {
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
                        text: 'Napi kiadások (aktuális hónap)'
                    }
                }
            }
        });
    }
});

document.addEventListener("DOMContentLoaded", () => {
    const totalElem = document.getElementById("totalValue");
    if (totalElem) {
        const target = parseFloat(totalElem.textContent);
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


