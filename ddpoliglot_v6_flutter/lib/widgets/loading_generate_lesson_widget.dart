import 'package:flutter/material.dart';

class LoadingGenerateLessonWidget extends StatelessWidget {
  const LoadingGenerateLessonWidget({Key? key, required this.loadingData})
      : super(key: key);
  final LoadingData loadingData;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.fromLTRB(15, 10, 15, 0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.start,
        mainAxisSize: MainAxisSize.max,
        children: [
          Container(
              padding: const EdgeInsets.only(top: 30),
              child: const Center(child: CircularProgressIndicator())),
          Container(
            padding: const EdgeInsets.only(top: 20),
            child: Center(
              child: Text(loadingData.message1),
            ),
          ),
          Container(
            padding: const EdgeInsets.only(top: 20),
            child: Center(
              child: loadingData.progress1 > 0
                  ? LinearProgressIndicator(
                      value: loadingData.progress1.toDouble() / 100,
                    )
                  : null,
            ),
          ),
          Container(
            padding: const EdgeInsets.only(top: 20),
            child: Center(
              child: Text(loadingData.message2),
            ),
          ),
          Container(
            padding: const EdgeInsets.only(top: 10),
            child: Center(
              child: loadingData.progress1 > 0
                  ? LinearProgressIndicator(
                      value: loadingData.progress2.toDouble() / 100,
                    )
                  : null,
            ),
          ),
        ],
      ),
    );
  }
}

class LoadingData {
  final String message1;
  final String message2;
  final int progress1;
  final int progress2;
  LoadingData(
      {this.message1 = '',
      this.message2 = '',
      this.progress1 = 0,
      this.progress2 = 0});
  LoadingData copyWith({
    String? message1,
    String? message2,
    int? progress1,
    int? progress2,
  }) =>
      LoadingData(
        message1: message1 ?? this.message1,
        message2: message2 ?? this.message2,
        progress1: progress1 ?? this.progress1,
        progress2: progress2 ?? this.progress2,
      );
}
