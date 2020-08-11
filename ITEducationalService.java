/*
 * Copyright 2013 Primeton Technologies Ltd.
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
import org.gocom.components.nui.demo.newdataset.TEducational;

/**
 * 完成个人学历的增删改查功能
 *
 * @author leijian (mailto:leijian@primeton.com)
 */
public interface ITEducationalService{

	/**
	 *
	 * @param tEducational TEducational
	 */
	void addTEducational(TEducational tEducational);

	/**
	 *
	 * @param tEducationals TEducational[]
	 */
	void deleteTEducational(TEducational[] tEducationals);

	/**
	 *
	 * @param tEducational TEducational[]
	 */
	void getTEducational(TEducational tEducational);

	/**
	 *
	 * @param criteria CriteriaType
	 * @param page PageCond
	 * @return TEducational[]
	 */
	TEducational[] queryTEducationals(CriteriaType criteriaType);

	/**
	 *
	 * @param tEducational TEducational[]
	 */
	void updateTEducational(TEducational tEducational);
	
}

