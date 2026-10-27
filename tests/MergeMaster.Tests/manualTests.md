- commit not pushed = success (404 intended response - custom error message not really a priority, since this is also checked by Claude before sending the request)
- commit duplicate = success (400 intended response with custom message)
- commit already merged = success (400 intended response with custom message)

