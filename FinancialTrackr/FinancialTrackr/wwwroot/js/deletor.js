const delbtn = document.querySelectorAll('.deletion');
const deleteModal = document.getElementById("myModal2");
const closeBtn = document.querySelector(".close2");
const deleteInput = document.getElementById("deleteUsername");

delbtn.forEach(btn => {
    btn.addEventListener('click', function () {
        deleteInput.value = this.dataset.username;

        deleteModal.style.display = "flex";
    });
});

closeBtn.addEventListener("click", () => {
    deleteModal.style.display = "none";
});

window.addEventListener("click", (e) => {
    if (e.target === deleteModal) {
        deleteModal.style.display = "none";
    }
});