provider "aws" {
  region  = "us-east-2"
  profile = "default"
}

resource "aws_dynamodb_table" "command_notifications" {
  name           = "command_notifications_tids"
  billing_mode   = "PROVISIONED"
  read_capacity  = 1
  write_capacity = 1

  hash_key = "messageId"
  attribute {
    name = "messageId"
    type = "S"
  }

  range_key = "createdTimestamp"
  attribute {
    name = "createdTimestamp"
    type = "S"
  }
}

resource "aws_sqs_queue" "sqs_notification_queue" {
  name                       = var.queue_name
  message_retention_seconds  = var.retention_period
  visibility_timeout_seconds = var.visibility_timeout
  redrive_policy = jsonencode({
    "deadLetterTargetArn" = aws_sqs_queue.sqs_notification_dlq.arn
    "maxReceiveCount"     = var.receive_count
  })
}

resource "aws_sqs_queue" "sqs_notification_dlq" {
  name                       = "${var.queue_name}-dlq"
  message_retention_seconds  = var.retention_period
  visibility_timeout_seconds = var.visibility_timeout
}