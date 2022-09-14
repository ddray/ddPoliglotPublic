import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';
import 'package:flutter/material.dart';

class InputNumberWidget extends StatelessWidget {
  const InputNumberWidget(
      {Key? key,
      required this.value,
      this.label = '',
      required this.onPlus,
      required this.onMinus})
      : super(key: key);
  final int value;
  final String label;
  final Function() onPlus;
  final Function() onMinus;

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        IconButton(
          onPressed: () => onMinus(),
          icon: const Icon(Icons.arrow_left),
          color: ColorsUtils.customYellowColor,
        ),
        if (label.isNotEmpty) Text(label),
        Text(value.toString()),
        IconButton(
            onPressed: () => onPlus(),
            icon: const Icon(Icons.arrow_right),
            color: ColorsUtils.customYellowColor)
      ],
    );
  }
}
