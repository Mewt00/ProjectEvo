// Darwin.h

#pragma once

#include <cstdlib>
#include <iostream>
#include <iomanip>
#include <fstream>
#include <cmath>
#include <ctime>
#include <cstring>
#include <list>
#include <iterator>
#include <math.h>
#include <vector>
#include <algorithm>

using std::ifstream;
using std::cout;
using std::cerr;
using std::setw;
using std::cin;
using std::endl;
using std::string;
using std::iterator;
using std::vector;

namespace Evolution {


	class Genotype
		{
			vector<int> gene;
			double fitness;
			double rFitness;
			double cFitness;


		public:
			Genotype();
			void assignGene ( vector<int>, vector<int> );
			vector<int> spliceGene (int, int);
			void genePushBack( int );
			vector<int>* getGene();
			double getFitness();
			double getRFitness();
			double getCFitness();
			void setGene(vector <int>);
			void setFitness(double);
			void setRFitness(double);
			void setCFitness(double);


		};

}