# Polar TFIDF
Polar TFIDF is a C# library for dealing with document keywords.

## Installation
Download .zip file from github and extract it to your solution.

## Usage
```cs
using Polar.ML.TfIdf;

var terms = new List<TermData>(){
  new TermData(){
    Count = 2,
    Term = "to"
  },
  new TermData(){
    Count = 2,
    Term = "be"
  },
  new TermData(){
    Count = 1,
    Term = "or"
  },
  new TermData(){
    Count = 1,
    Term = "not"
  }
};

TfIdfEstimator.AddDocument("Hamlet", terms);
```

## FAQ about the source code

#### Do we get support when we encounter problem or issue when working with the source code?
Yes. Contact us at: support@polarsoftware.com.

#### What about newer versions of the source code? Will they be available when new releases come out?
Yes.

## Licence
The licence agreement can be found in [LICENCE.md](https://github.com/polarsoftware/PolarTFIDF/blob/master/LICENCE.md) file.

## Buy Polar TFIDF
You can purchase at:
  
[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_buynowCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=5GXZ8B4QAT2EW)
