class HttpException1 {
  final String message;

  HttpException1(this.message);

  @override
  String toString() {
    return message;
    // return super.toString(); // Instance of HttpException
  }
}
