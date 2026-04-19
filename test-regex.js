function testEmail(email) {
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    console.log(email + ": " + emailPattern.test(email));
}

testEmail('abhi@eza.co');
testEmail('abhi@yopmail.comdsfdsf');
testEmail('abhi@');
