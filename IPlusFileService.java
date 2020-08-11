/*
 * Copyright 2013 Primeton.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package org.gocom.components.nui.demo;

import com.eos.das.entity.criteria.CriteriaType;


import org.gocom.components.nui.sample.ntree.PlusFile;

/**
 * 实现树形控件(节点)的增删改查
 * 
 * @author huangct (mailto:huangct@primeton.com)
 */
public interface IPlusFileService {

	/**
	 * 
	 * @param plusFile
	 *            PlusFile
	 */
	void addPlusFile(PlusFile plusFile);

	/**
	 * 
	 * @param plusFiles
	 *            PlusFile[]
	 */
	void deletePlusFile(PlusFile[] plusFiles);

	/**
	 * 
	 * @param plusFile
	 *            PlusFile[]
	 */
	void getPlusFile(PlusFile plusFile);

	/**
	 * 
	 * @param criteria
	 *            CriteriaType
	 * @param page
	 *            PageCond
	 * @return PlusFile[]
	 */
	PlusFile[] queryPlusFiles(CriteriaType criteriaType, int begin, int length);

	/**
	 * 
	 * @param criteria
	 *            CriteriaType
	 * @param page
	 *            PageCond
	 * @return 
	 */
	int countFiles(CriteriaType criteria);

	/**
	 * 
	 * @param plusFile
	 *            PlusFile[]
	 */
	void updatePlusFile(PlusFile plusFile);
	
	/**
	 * 
	 * @param plusFile
	 *            PlusFile[]
	 */
	void saveTree(PlusFile[] plusFile);
	
	PlusFile[] loadNodes(String id);

}
