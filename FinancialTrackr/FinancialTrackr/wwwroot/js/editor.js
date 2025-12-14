const modalBtns = document.querySelectorAll('.js-btn');
const modal = document.getElementById("myModal");
const span = document.querySelector(".close");

const usernameField = document.getElementById("modalUsername");
const newUsernameField = document.getElementById("modalNewUsername");
const newEmailField = document.getElementById("modalNewEmail");
const isAdminField = document.getElementById("modalIsAdmin");


modalBtns.forEach(btn => {
    btn.addEventListener('click', function () {
        const modal = document.getElementById("myModal");
        const span = document.querySelector(".close");

        modal.style.display = "flex";
        
        //`
        //<form method="post" asp-page-handler="EditUserCredentials">
        //    <label>Felhasználónév változtatása<label>
        //    <input asp-for="Username" type="Text">
        //    <label>Email Cím Változtatása<label>
        //    <input asp-for="Email" type="Text">
        //    <label>Admin jog változtatása (1 = igen, 0 = nem)<label>
        //    <input asp-for="isUserAnAdmin" type="number">
        //    <button type="Submit">Változtatások mentése</button>
        //</form>
        
        
        //`
        span.addEventListener("click", () => {
            modal.style.display = "none";
        });

        window.addEventListener("click", (e) => {
            if (e.target === modal) {
                modal.style.display = "none";
            }
        });
    });
});
    



  

