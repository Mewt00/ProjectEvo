#include "Genotype.h"

using namespace Evolution;


	Genotype::Genotype()
	{
		fitness = 0;
		rFitness = 0;
		cFitness = 0;
	}
	
	//			A	  B
	//point -- [XXX[P)XX)
	//
	void Genotype::assignGene ( vector<int> a, vector<int> b )
	{
		gene.assign(a.begin(), a.end());
		gene.insert(gene.end(), b.begin(), b.end());
	}
	
	vector<int> Genotype::spliceGene (int a, int b)
	{
		std::vector<int>::iterator firstIterator = gene.begin() + a;
		std::vector<int>::iterator nextIterator = gene.begin() + b;

		vector<int> v; 
		v.assign(firstIterator, nextIterator);
		return v;
	}

	void Genotype::genePushBack( int g )
	{
		gene.push_back( g );
		return;
	}

	vector<int>* Genotype::getGene()			
	{
		return &gene;
	}
			
	double Genotype::getFitness()
	{
		return fitness;
	}
			
	double Genotype::getRFitness()
	{
		return rFitness;
	}
			
	double Genotype::getCFitness()
	{
		return cFitness;
	}

	void Genotype::setGene(vector <int> g)
	{
		gene = g;
		return;
	}

	void Genotype::setFitness(double f)
	{
		fitness = f;
	}
			
	void Genotype::setRFitness(double r)
	{
		rFitness = r;
	}
			
	void Genotype::setCFitness(double c)
	{
		cFitness = c;
	}