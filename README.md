# Email Sending System with RabbitMQ

**Email Sending System with RabbitMQ** is a messaging-based email delivery system that utilizes RabbitMQ as a message broker to send emails asynchronously.
## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)

## Features

- **Scalability**: The system can be easily scaled by adding more workers as needed to handle email sending tasks.
- **Configuration**: Flexible configuration options to adapt to various email service providers.
- **Large Attachment Support**: You can customize app to modify it's attachment size support.

## Prerequisites

Before you begin, ensure you have met the following requirements:

- RabbitMQ server is installed and running.

## Installation

To set up the Email Sending System with RabbitMQ, follow these steps:

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/Raienryuu/RabbitMQ-test.git
   ```
   
2. **Configure connection settings**
   Set valid RabbitMq address for the system to work properly.


## Usage
To use the Email Sending System, you need to publish messages to the RabbitMQ exchange with the necessary email content. The system's worker process will pick up these messages and send the emails asynchronously.
You can integrate this system into your applications by publishing email tasks to RabbitMQ. Make sure to follow the messaging format expected by the worker process.
