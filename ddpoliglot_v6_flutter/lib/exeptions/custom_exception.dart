class CustomException implements Exception {
  String message;
  final String _screenMessage;
  CustomException(
      [this.message = 'Something went wrong', this._screenMessage = '']) {
    message = message;
  }

  String get screenMessage => _screenMessage;

  @override
  String toString() {
    return message;
  }
}
