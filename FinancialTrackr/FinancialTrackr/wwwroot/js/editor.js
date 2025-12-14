document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("myModal");
    const closeBtn = modal.querySelector(".close");

    document.querySelectorAll(".js-btn").forEach(btn => {
        btn.addEventListener("click", () => {
            // Fill hidden + visible inputs
            document.getElementById("modalUsername").value = btn.dataset.username;
            document.getElementById("modalNewUsername").value = btn.dataset.username;
            document.getElementById("modalNewEmail").value = btn.dataset.email;
            document.getElementById("modalIsAdmin").value = btn.dataset.admin;

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
