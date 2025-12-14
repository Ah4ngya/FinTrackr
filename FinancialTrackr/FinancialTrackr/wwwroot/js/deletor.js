document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("myModal2");
    const closeBtn = modal.querySelector(".close2");
    const hiddenInput = document.getElementById("deleteUsername");

    document.querySelectorAll(".deletion").forEach(btn => {
        btn.addEventListener("click", () => {
            hiddenInput.value = btn.dataset.username;
            modal.style.display = "flex";
        });
    });

    closeBtn.addEventListener("click", () => {
        modal.style.display = "none";
    });

    window.addEventListener("click", e => {
        if (e.target === modal) {
            modal.style.display = "none";
        }
    });
});
