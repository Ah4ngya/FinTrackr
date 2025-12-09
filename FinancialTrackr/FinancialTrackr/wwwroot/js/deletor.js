const delbtn = document.querySelectorAll('.deletion');
const deleteModal = document.getElementById("myModal2");
const closeBtn = document.querySelector(".close2");
const deleteInput = document.getElementById("deleteUsername");

delbtn.forEach(btn => {
    btn.addEventListener('click', function () {
        // Fill hidden input with the username
        deleteInput.value = this.dataset.username;

        // Show modal
        deleteModal.style.display = "block";
    });
});

// Close modal when X is clicked
closeBtn.addEventListener("click", () => {
    deleteModal.style.display = "none";
});

// Close modal when clicking outside of it
window.addEventListener("click", (e) => {
    if (e.target === deleteModal) {
        deleteModal.style.display = "none";
    }
});