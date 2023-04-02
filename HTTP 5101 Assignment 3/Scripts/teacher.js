function ValidateTeacher() {

	var IsValid = true;
	var ErrorMsg = "";
	var ErrorBox = document.getElementById("ErrorBox");
	var TeacherFname = document.getElementById('TeacherFname').value;
	var TeacherLname = document.getElementById('TeacherLname').value;
	var TeacherEmployeeNumber = document.getElementById('TeacherEmployeeNumber').value;
	var TeacherHireDate = document.getElementById('TeacherHireDate').value;
	var TeacherSalary = document.getElementById('TeacherSalary').value;

	//First Name is two or more characters
	if (TeacherFname.length < 2) {
		IsValid = false;
		ErrorMsg += "First Name Must be 2 or more characters.<br>";
	}
	//Last Name is two or more characters
	if (TeacherLname.length < 2) {
		IsValid = false;
		ErrorMsg += "Last Name Must be 2 or more characters.<br>";
	}
	//Email is valid pattern
	if (!TeacherEmployeeNumber) {
		IsValid = false;
		ErrorMsg += "Please Enter a Employee Number.<br>";
	}
	if (!TeacherHireDate) {
		IsValid = false;
		ErrorMsg += "Please Select a Hire Date.<br>";
	}
	if (!TeacherSalary) {
		IsValid = false;
		ErrorMsg += "Please Enter a Salary.<br>";
	}

	if (!IsValid) {
		ErrorBox.style.display = "block";
		ErrorBox.innerHTML = ErrorMsg;
	} else {
		ErrorBox.style.display = "none";
		ErrorBox.innerHTML = "";
	}


	return IsValid;
}